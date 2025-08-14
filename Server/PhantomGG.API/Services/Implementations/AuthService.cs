using BCrypt.Net;
using PhantomGG.API.DTOs.Auth;
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

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
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

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = new DTOs.User.UserDto
            {
                Id = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                FullName = createdUser.FullName,
                Email = createdUser.Email,
                Role = userRole.Name,
                CreatedAt = createdUser.CreatedAt
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Get user role
        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        if (role == null)
        {
            throw new InvalidOperationException("User role not found");
        }

        // Revoke existing refresh tokens (optional - for single session)
        // await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id);

        // Generate new tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, role.Name);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = new DTOs.User.UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                Role = role.Name,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshRequest request)
    {
        // Find refresh token
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (refreshToken == null || !refreshToken.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Get user and role
        var user = refreshToken.User;
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        if (role == null)
        {
            throw new InvalidOperationException("User role not found");
        }

        // Generate new access token
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, role.Name);

        // Optionally generate new refresh token and revoke old one
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Revoke old refresh token
        await _refreshTokenRepository.RevokeAsync(request.RefreshToken);

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(newRefreshTokenEntity);

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        try
        {
            await _refreshTokenRepository.RevokeAsync(token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RevokeAllUserTokensAsync(Guid userId)
    {
        try
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task CleanupExpiredTokensAsync()
    {
        await _refreshTokenRepository.CleanupExpiredTokensAsync();
    }
}
