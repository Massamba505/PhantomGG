using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenService refreshTokeService,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ICookieService cookieService,
    IEmailService emailService,
    IAuthVerificationService authVerificationService,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IAuthVerificationService _authVerificationService = authVerificationService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IRefreshTokenService _refreshTokeService = refreshTokeService;
    private readonly ICookieService _cookieService = cookieService;
    private readonly IEmailService _emailService = emailService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task RegisterAsync(RegisterRequestDto request)
    {
        await ValidateUniqueEmailAsync(request.Email);
        string passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = request.ToEntity(passwordHash);

        await _userRepository.CreateAsync(user);
        await _authVerificationService.ResendEmailVerificationAsync(user.Email);
    }

    public async Task<AuthDto> LoginAsync(LoginRequestDto request)
    {
        var user = await ValidateUserLoginAsync(request.Email, request.Password);

        await HandleSuccessfulLoginAsync(user);
        return await GenerateTokensAsync(user, rememberMe: request.RememberMe);
    }

    private async Task<User> ValidateUserLoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email.ToLower());

        if (user == null || !user.IsActive)
        {
            await HandleFailedLoginAsync(email.ToLower());
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

        var validPassword = _passwordHasher.VerifyPassword(password, user.PasswordHash);
        if (!validPassword)
        {
            await HandleFailedLoginAsync(user);
            throw new UnauthorizedException("Invalid email or password");
        }

        return user;
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

    private async Task ValidateUniqueEmailAsync(string email)
    {
        var emailExist = await _userRepository.EmailExistsAsync(email.ToLower());
        if (emailExist)
        {
            throw new ConflictException("Email address is already registered");
        }
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
