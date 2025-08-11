using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;

using System.Security.Claims;

namespace PhantomGG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IIdentityAuthentication identityAuth,
    ICookieService cookieService
    ) : ControllerBase
{
    private readonly IIdentityAuthentication _identityAuth = identityAuth;
    private readonly ICookieService _cookieService = cookieService;

    /// <summary>
    /// Registers a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _identityAuth.RegisterAsync(request);

        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.Message });
        }

        _cookieService.SetAuthCookies(Response, new TokenResponse
        {
            AccessToken = result.AccessToken ?? string.Empty,
            RefreshToken = result.RefreshToken ?? string.Empty,
            AccessTokenExpires = result.AccessTokenExpires ?? DateTime.UtcNow,
            RefreshTokenExpires = result.RefreshTokenExpires ?? DateTime.UtcNow
        }, true);

        return Ok(new
        {
            success = true,
            accessToken = result.AccessToken,
            user = result.User,
            accessTokenExpires = result.AccessTokenExpires
        });
    }

    /// <summary>
    /// Authenticates a user
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _identityAuth.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(new { success = false, message = result.Message });
        }

        _cookieService.SetAuthCookies(Response, new TokenResponse
        {
            AccessToken = result.AccessToken ?? string.Empty,
            RefreshToken = result.RefreshToken ?? string.Empty,
            AccessTokenExpires = result.AccessTokenExpires ?? DateTime.UtcNow,
            RefreshTokenExpires = result.RefreshTokenExpires ?? DateTime.UtcNow
        }, request.RememberMe);

        return Ok(new
        {
            success = true,
            message = "Login Successfully",
            accessToken = result.AccessToken,
            user = result.User,
            accessTokenExpires = result.AccessTokenExpires
        });
    }

    /// <summary>
    /// Refreshes an expired access token
    /// </summary>
    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest? request = null)
    {
        var refreshToken = "";

        if (Request.Cookies.TryGetValue("refreshToken", out var cookieToken))
        {
            refreshToken = cookieToken;
        }

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { success = false, message = "Refresh token is required" });
        }

        var result = await _identityAuth.RefreshTokenAsync(refreshToken);

        if (!result.Success)
        {
            return Unauthorized(new { success = false, message = result.Message });
        }

        bool persistCookie = request?.PersistCookie ?? true;

        _cookieService.SetAuthCookies(Response, new TokenResponse
        {
            AccessToken = result.AccessToken ?? string.Empty,
            RefreshToken = result.RefreshToken ?? string.Empty,
            AccessTokenExpires = result.AccessTokenExpires ?? DateTime.UtcNow,
            RefreshTokenExpires = result.RefreshTokenExpires ?? DateTime.UtcNow
        }, persistCookie);

        return Ok(new
        {
            success = true,
            message = "Refreshed Successfully",
            accessToken = result.AccessToken,
            accessTokenExpires = result.AccessTokenExpires
        });
    }

    /// <summary>
    /// Logs out the current user
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            await _identityAuth.LogoutAsync(refreshToken);
        }

        _cookieService.ClearAuthCookies(Response);

        return Ok(new
        {
            success = true,
            message = "Logged out successfully"
        });
    }

    /// <summary>
    /// Gets the current user's profile
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { success = false, message = "Invalid user" });
        }

        var userProfile = await _identityAuth.GetCurrentUserAsync(userId);
        if (userProfile == null)
        {
            return Unauthorized(new { success = false, message = "User not found" });
        }

        return Ok(new
        {
            success = true,
            message = "Login Successfully",
            user = userProfile
        });
    }
}