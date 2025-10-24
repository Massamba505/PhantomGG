using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using PhantomGG.Common.Config;
using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Service.Auth.Interfaces;
using PhantomGG.Service.Domain.Users.Interfaces;
using PhantomGG.Service.Infrastructure.Security.Interfaces;

namespace PhantomGG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    ICookieService cookieService,
    IOptions<CookieSettings> cookieSettings,
    ICurrentUserService currentUserService,
    IUserService userService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ICookieService _cookieService = cookieService;
    private readonly CookieSettings _cookieSettings = cookieSettings.Value;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [EnableRateLimiting("RegisterPolicy")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        await _authService.RegisterAsync(request);

        return Accepted(new { message = "Verification email sent. Please check your inbox." });

    }

    /// <summary>
    /// Authenticate user credentials and login
    /// </summary>

    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token from cookie
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh()
    {
        var refreshToken = Request.Cookies[_cookieSettings.RefreshTokenCookieName] ?? string.Empty;
        var result = await _authService.RefreshAsync(refreshToken);
        return Ok(new RefreshTokenResponse(result.AccessToken));
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me()
    {
        var currentUser = _currentUserService.GetCurrentUser()!;
        var result = await _userService.GetByIdAsync(currentUser.Id);
        return Ok(result);
    }

    /// <summary>
    /// Logout user by clearing refresh token cookie
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        var refreshToken = Request.Cookies[_cookieSettings.RefreshTokenCookieName];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.LogoutAsync(refreshToken);
        }

        _cookieService.ClearRefreshToken(Response);
        return NoContent();
    }
}
