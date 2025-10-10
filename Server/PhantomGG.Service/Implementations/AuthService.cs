using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace PhantomGG.Service.Implementations;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenService refreshTokeService,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ICookieService cookieService,
    IEmailService emailService,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IRefreshTokenService _refreshTokeService = refreshTokeService;
    private readonly ICookieService _cookieService = cookieService;
    private readonly IEmailService _emailService = emailService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task RegisterAsync(RegisterRequestDto request)
    {
        ValidateRegisterRequest(request);

        var emailExist = await _userRepository.EmailExistsAsync(request.Email.ToLower());
        if (emailExist)
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
            Role = (int)request.Role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            EmailVerified = false,
            EmailVerificationToken = GenerateSecureToken(),
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1),
            FailedLoginAttempts = 0
        };

        await _userRepository.CreateAsync(user);

        await _emailService.SendEmailVerificationAsync(user.Email, user.FirstName, user.EmailVerificationToken!);
    }

    public async Task<AuthDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLower());
        if (user == null || !user.IsActive)
        {
            await HandleFailedLoginAsync(request.Email.ToLower());
            throw new UnauthorizedException("Invalid email or password");
        }

        if (user.AccountLockedUntil.HasValue && user.AccountLockedUntil > DateTime.UtcNow)
        {
            throw new UnauthorizedException($"Account is locked until {user.AccountLockedUntil:yyyy-MM-dd HH:mm} UTC");
        }

        if (!user.EmailVerified)
        {
            throw new UnauthorizedException("Please verify your email address");
        }

        var validPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!validPassword)
        {
            await HandleFailedLoginAsync(user);
            throw new UnauthorizedException("Invalid email or password");
        }

        await HandleSuccessfulLoginAsync(user);
        return await GenerateTokensAsync(user, rememberMe: request.RememberMe);
    }

    public async Task<AuthDto> RefreshAsync(string refreshTokenFromCookie)
    {
        var deletedRefreshToken = await _refreshTokeService.DeleteAsync(refreshTokenFromCookie);
        var user = deletedRefreshToken.User;

        return await GenerateTokensAsync(user);
    }

    public async Task LogoutAsync(string refreshTokenFromCookie)
    {
        await _refreshTokeService.DeleteAsync(refreshTokenFromCookie);
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

        if (!IsValidPassword(request.Password))
        {
            errors.Add("Password must be at least 8 characters with uppercase, lowercase, number, and special character");
        }

        if (errors.Any())
        {
            throw new ValidationException(string.Join("; ", errors));
        }
    }

    private static bool IsValidPassword(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$");
    }

    private async Task<AuthDto> GenerateTokensAsync(User user, bool rememberMe = true)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = await _refreshTokeService.AddRefreshToken(user);
        var result = new AuthDto
        {
            AccessToken = accessToken.Token,
            User = user.ToDto()
        };

        _cookieService.SetRefreshToken(_httpContextAccessor.HttpContext!.Response, refreshToken.Token, rememberMe);

        return result;
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    private async Task HandleFailedLoginAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user != null)
        {
            await HandleFailedLoginAsync(user);
        }
    }

    private async Task HandleFailedLoginAsync(User user)
    {
        user.FailedLoginAttempts++;

        if (user.FailedLoginAttempts >= 5)
        {
            user.AccountLockedUntil = DateTime.UtcNow.AddMinutes(30);
            await _emailService.SendAccountLockedAsync(user.Email, user.FirstName, user.AccountLockedUntil.Value);
        }

        await _userRepository.UpdateAsync(user);
    }

    private async Task HandleSuccessfulLoginAsync(User user)
    {
        user.FailedLoginAttempts = 0;
        user.AccountLockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
    }
}
