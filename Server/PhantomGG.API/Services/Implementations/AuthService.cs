using PhantomGG.API.Common;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.RefreshToken;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepo,
        IRefreshTokenService refreshTokenService,
        IPasswordService passwordService,
        ITokenService tokenService,
        ICookieService cookieService,
        ICurrentUserService currentUserService,
        IRefreshTokenRepository refreshTokenRepo,
        ILogger<AuthService> logger)
    {
        _userRepo = userRepo;
        _refreshTokenService = refreshTokenService;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _cookieService = cookieService;
        _logger = logger;
        _currentUserService = currentUserService;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepo.EmailExistsAsync(request.Email))
            return new AuthResponse { Success = false, Message = "Email already registered" };

        var passwordHashResult = _passwordService.CreatePasswordHash(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHashResult.Hash,
            PasswordSalt = passwordHashResult.Salt,
            ProfilePictureUrl = $"https://eu.ui-avatars.com/api/?name={request.FirstName}+{request.LastName}&size=250",
            Role = UserRoles.Organizer.ToString(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepo.CreateAsync(user);

        var tokens = await GenerateTokensAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Registration successful",
            User = ToProfileDto(user),
            Tokens = tokens
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);

        if (user == null )
        {
            return new AuthResponse { Success = false, Message = "Invalid credentials" };
        }
        
        if (!user.IsActive)
        {
            return new AuthResponse { Success = false, Message = "User account is not active" };
        }

        var isPasswordValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
        {
            return new AuthResponse { Success = false, Message = "Invalid credentials" };
        }
        var tokens_refresh = await _refreshTokenRepo.GetByUserIdAsync(user.Id);
        if (tokens_refresh != null)
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(tokens_refresh.Id);
        }

        var tokens = await GenerateTokensAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            User = ToProfileDto(user),
            Tokens = tokens
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var token = await _refreshTokenService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (token == null)
        {
            return new AuthResponse { Success = false, Message = "Invalid refresh token" };
        }

        var user = token.User;
        if (user == null || !user.IsActive)
        {            
            return new AuthResponse { Success = false, Message = "User not found" };
        }

        await _refreshTokenService.RevokeRefreshTokenAsync(token.Id);

        var newTokens = await GenerateTokensAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Token refreshed",
            User = ToProfileDto(user),
            Tokens = newTokens
        };
    }

    public async Task LogoutAsync()
    {
        var userId = _currentUserService.UserId!;
        var refreshtoken = await _refreshTokenRepo.GetByUserIdAsync(userId.Value);
        if (refreshtoken != null)
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshtoken.Id);
        }
    }

    public async Task<TokenPair> GenerateTokensAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _refreshTokenService.CreateRefreshTokenAsync(user.Id, refreshToken);

        return new TokenPair
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public UserProfileDto ToProfileDto(User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}