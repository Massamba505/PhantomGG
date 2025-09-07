using PhantomGG.API.Common;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Mappings;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PhantomGG.API.Services.Implementations;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokeService refreshTokeService,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ICookieService cookieService,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IRefreshTokeService _refreshTokeService = refreshTokeService;
    private readonly ICookieService _cookieService = cookieService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<AuthDto> RegisterAsync(RegisterRequestDto request)
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
            Role = request.Role.ToString(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLower());
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        var validPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!validPassword)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        return await GenerateTokensAsync(user, rememberMe:request.RememberMe);
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

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character");
        }
        else if (!IsValidPassword(request.Password))
        {
            errors.Add("Password must contain at least one letter and one digit");
        }

        if(!EnumHelper.ToEnum<UserRoles>(request.Role).HasValue)
        {
            errors.Add("Invalid role");
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
            User = user.ToUserDto()
        };

        _cookieService.SetRefreshToken(_httpContextAccessor.HttpContext!.Response, refreshToken.Token, rememberMe);

        return result;
    }
}
