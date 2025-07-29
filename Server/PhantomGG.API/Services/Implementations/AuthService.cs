using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResult> RegisterUserAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            return new AuthResult(false, "Email already registered");

        var passwordResult = _passwordService.CreatePasswordHash(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordResult.Hash,
            PasswordSalt = passwordResult.Salt,
            ProfilePictureUrl = $"https://eu.ui-avatars.com/api/?name={request.FirstName}+{request.LastName}&size=250",
            Role = "Organizer",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);
        return new AuthResult(true, "Registration successful");
    }

    public async Task<AuthResult> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.IsActive)
            return new AuthResult(false, "Invalid credentials");

        if (!_passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            return new AuthResult(false, "Invalid credentials");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

        return new AuthResult(true, "Login successful", accessToken, refreshToken);
    }

    public async Task<AuthResult> RefreshTokensAsync(string refreshToken)
    {
        var validToken = await _refreshTokenService.ValidateTokenAsync(refreshToken);
        if (validToken == null)
            return new AuthResult(false, "Invalid refresh token");

        await _refreshTokenService.RevokeTokenAsync(validToken.Id);

        var newAccessToken = _tokenService.GenerateAccessToken(validToken.User);
        var newRefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(validToken.UserId);

        return new AuthResult(true, "Tokens refreshed", newAccessToken, newRefreshToken);
    }
}