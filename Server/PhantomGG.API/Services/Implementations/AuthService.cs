using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.Auth.Requests;
using PhantomGG.API.DTOs.Auth.Responses;
using PhantomGG.API.DTOs.User.Responses;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        ICookieService cookieService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _cookieService = cookieService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DTOs.Auth.Responses.AuthResponse> RegisterAsync(DTOs.Auth.Requests.RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Check if name is taken
        var existingName = await _userRepository.GetByNameAsync(request.FirstName, request.LastName);
        if (existingName != null)
        {
            throw new InvalidOperationException("User with this name already exists");
        }

        // Get default role (User)
        var userRole = await _roleRepository.GetByNameAsync("User");
        if (userRole == null)
        {
            throw new InvalidOperationException("Default user role not found");
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = userRole.Id,
            EmailConfirmed = false,
            IsActive = true,
            FailedLoginAttempts = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(createdUser.Id, createdUser.Email, userRole.Name);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = createdUser.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        // Create token response
        var tokenResponse = new DTOs.Auth.Responses.TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpires = refreshTokenEntity.ExpiresAt
        };

        _cookieService.SetAuthCookies(_httpContextAccessor.HttpContext!.Response, tokenResponse, false);

        return new AuthResponse
        {
            AccessToken = accessToken,
            User = new UserDto
            {
                Id = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                FullName = $"{createdUser.FirstName} {createdUser.LastName}",
                Email = createdUser.Email,
                Role = userRole.Name,
                CreatedAt = createdUser.CreatedAt
            }
        };
    }

    public async Task<DTOs.Auth.Responses.AuthResponse> LoginAsync(DTOs.Auth.Requests.LoginRequest request)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailWithRoleAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if user is locked out
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Account is temporarily locked. Please try again later.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            // Increment failed login attempts
            await _userRepository.IncrementFailedLoginAttemptsAsync(user.Id);

            // Check if we should lock the account (after 5 failed attempts)
            if (user.FailedLoginAttempts >= 4) // It will become 5 after the increment
            {
                await _userRepository.SetLockoutAsync(user.Id, DateTime.UtcNow.AddMinutes(15));
            }

            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Reset failed login attempts
        await _userRepository.ResetFailedLoginAttemptsAsync(user.Id);

        // Revoke any existing tokens if not remembered
        var existingTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(user.Id);
        foreach (var token in existingTokens)
        {
            // Only revoke tokens that weren't created with RememberMe
            if (token.ExpiresAt < DateTime.UtcNow.AddDays(7))
            {
                token.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(token);
            }
        }

        // Generate new tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.Name);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = request.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        // Create token response
        var tokenResponse = new DTOs.Auth.Responses.TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpires = refreshTokenEntity.ExpiresAt
        };

        _cookieService.SetAuthCookies(_httpContextAccessor.HttpContext!.Response, tokenResponse, request.RememberMe);

        return new AuthResponse
        {
            AccessToken = accessToken,
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.Role.Name,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<DTOs.Auth.Responses.TokenResponse> RefreshTokenAsync()
    {
        // Get token from cookie
        var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new InvalidOperationException("Invalid or missing refresh token.");
        }

        // Validate refresh token
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (token == null || token.RevokedAt != null || token.ExpiresAt < DateTime.UtcNow)
        {
            _cookieService.ClearAuthCookies(_httpContextAccessor.HttpContext!.Response);
            throw new InvalidOperationException("Invalid or expired refresh token.");
        }

        // Get user with role
        var user = await _userRepository.GetByIdWithRoleAsync(token.UserId);
        if (user == null)
        {
            _cookieService.ClearAuthCookies(_httpContextAccessor.HttpContext!.Response);
            throw new InvalidOperationException("User not found.");
        }

        // Determine if this was a "remember me" token by looking at expiration date
        // If it expires more than 2 days from creation, it was a "remember me" token
        bool rememberMe = (token.ExpiresAt - token.CreatedAt).TotalDays > 2;

        // Generate new tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.Name);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Revoke old token
        token.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(token);

        // Save new refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        // Create token response
        var tokenResponse = new DTOs.Auth.Responses.TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpires = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpires = refreshTokenEntity.ExpiresAt
        };

        // Set refresh token in cookie
        _cookieService.SetAuthCookies(_httpContextAccessor.HttpContext!.Response, tokenResponse, rememberMe);

        return tokenResponse;
    }

    public async Task<LogoutResponse> RevokeTokenAsync()
    {
        // Get token from cookie
        var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return new LogoutResponse { Success = false, Message = "No refresh token found." };
        }

        // Remove cookie
        _cookieService.ClearAuthCookies(_httpContextAccessor.HttpContext!.Response);

        // Revoke token in database
        await _refreshTokenRepository.RevokeAsync(refreshToken);

        return new LogoutResponse { Success = true, Message = "Successfully logged out." };
    }

    public async Task<LogoutResponse> RevokeAllUserTokensAsync(Guid userId)
    {
        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        _cookieService.ClearAuthCookies(_httpContextAccessor.HttpContext!.Response);

        return new LogoutResponse { Success = true, Message = "Successfully revoked all tokens." };
    }

    public async Task CleanupExpiredTokensAsync()
    {
        await _refreshTokenRepository.CleanupExpiredTokensAsync();
    }

    public async Task<AuthResponse> GetCurrentUserAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            throw new InvalidOperationException("User not authenticated.");
        }

        var user = await _userRepository.GetByIdWithRoleAsync(userId.Value);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        return new AuthResponse
        {
            AccessToken = "",  // Client already has this token
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.Role.Name,
                CreatedAt = user.CreatedAt
            }
        };
    }

    private Guid? GetCurrentUserId()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return null;
        }

        var token = authHeader.Substring("Bearer ".Length);
        return _tokenService.GetUserIdFromToken(token);
    }
}
