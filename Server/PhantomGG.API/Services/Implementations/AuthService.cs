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
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenService refreshTokenService,
        IPasswordService passwordService,
        ITokenService tokenService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _refreshTokenService = refreshTokenService;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _currentUserService = currentUserService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        bool isEmailDuplicate = await _userRepository.EmailExistsAsync(request.Email);
        if (isEmailDuplicate)
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

        await _userRepository.CreateAsync(user);

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
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !user.IsActive || !_passwordService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return new AuthResponse { Success = false, Message = "Invalid credentials" };
        }

        var tokens_refresh = await _refreshTokenService.GetByUserIdAsync(user.Id);
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
        var refreshtoken = await _refreshTokenService.GetByUserIdAsync(userId.Value);
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