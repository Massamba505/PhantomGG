using PhantomGG.API.Common;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Utils;

namespace PhantomGG.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _tokenRepository;
    private readonly JwtUtils _jwtUtils;
    private readonly IPasswordService _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository tokenRepository,
        JwtUtils jwtUtils,
        ITokenService tokenService,
        IPasswordService passwordHasher,
        ICookieService cookieService)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _jwtUtils = jwtUtils;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _cookieService = cookieService;
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        bool isEmailDuplicate = await _userRepository.EmailExistsAsync(request.Email);
        if (isEmailDuplicate)
        {
            throw new Exception("Email already exists");
        }
            
        PasswordHashResult passwordHash = _passwordHasher.CreatePasswordHash(request.Password);
        
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash.Hash,
            PasswordSalt = passwordHash.Salt,
            ProfilePictureUrl = $"https://eu.ui-avatars.com/api/?name={request.FirstName}+{request.LastName}&size=250",
            Role = UserRoles.Organizer.ToString(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user);

        return user;
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("Invalid credentials");
        }

        bool isValidPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);

        if (!isValidPassword)
        {
            throw new Exception("Invalid credentials");
        }

        return user;
    }

    public async Task<TokenPair> RefreshTokenAsync(string refreshToken)
    {
        var tokenHash = _jwtUtils.HashRefreshToken(refreshToken);
        var token = await _tokenRepository.GetByTokenHashAsync(tokenHash);

        if (token == null || token.Expires < DateTime.UtcNow || token.IsRevoked) { 
            throw new Exception("Invalid refresh token");
        }

        return await _tokenService.GenerateAuthResponseAsync(token.User);
    }

    public async Task RevokeRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var tokenHash = _jwtUtils.HashRefreshToken(refreshToken);
        var token = await _tokenRepository.GetByTokenHashAsync(tokenHash);

        if (token == null)
        {
            throw new Exception("Refresh token not found");
        }

        if (token.UserId != userId)
        {
            throw new UnauthorizedAccessException("Not authorized to revoke this token");
        }

        await _tokenRepository.RevokeAsync(token);
    }
}