using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Mappings;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PhantomGG.API.Services.Implementations;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokeService refreshTokeService,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IRefreshTokeService _refreshTokeService = refreshTokeService;

    public async Task<AuthDto> RegisterAsync(RegisterRequestDto request)
    {
        ValidateRegisterRequest(request);

        if (await _userRepository.EmailExistsAsync(request.Email.ToLower()))
        {
            throw new ConflictException("Email address is already registered");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            ProfilePictureUrl = request.ProfilePictureUrl ?? $"https://eu.ui-avatars.com/api/?name={request.FirstName}+{request.LastName}&size=250",
            Role = "Organizer",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = await _refreshTokeService.AddRefreshToken(user);

        return new AuthDto
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            User = user.ToUserDto()
        };
    }

    public async Task<AuthDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLower());
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = await _refreshTokeService.AddRefreshToken(user);

        return new AuthDto
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            User = user.ToUserDto()
        };
    }

    public async Task<AuthDto> RefreshAsync(string refreshTokenFromCookie)
    {
        var revokedRefreshToken = await _refreshTokeService.RevokeAsync(refreshTokenFromCookie);
        var user = revokedRefreshToken.User;
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = await _refreshTokeService.AddRefreshToken(user);

        return new AuthDto
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            User = user.ToUserDto()
        };
    }


    public async Task LogoutAsync(string refreshTokenFromCookie)
    {
        await _refreshTokeService.RevokeAsync(refreshTokenFromCookie);
    }

    private static void ValidateRegisterRequest(RegisterRequestDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName) || request.FirstName.Length > 50)
        {
            errors.Add("FirstName is required and must be 1-50 characters");
        }

        if (string.IsNullOrWhiteSpace(request.LastName) || request.LastName.Length > 50)
        {
            errors.Add("LastName is required and must be 1-50 characters");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 100)
        {
            errors.Add("Email is required and must be valid and no more than 100 characters");
        }
        else if (!IsValidEmail(request.Email))
        {
            errors.Add("Email format is invalid");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character");
        }
        else if (!IsValidPassword(request.Password))
        {
            errors.Add("Password must contain at least one letter and one digit");
        }

        if (errors.Any())
        {
            throw new ValidationException(string.Join("; ", errors));
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPassword(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$");
    }
}
