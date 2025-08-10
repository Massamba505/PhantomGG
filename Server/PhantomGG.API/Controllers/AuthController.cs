using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Services.Interfaces;
using System.Net;
using System.Security.Claims;

namespace PhantomGG.API.Controllers;

/// <summary>
/// Controller for authentication-related endpoints
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityAuthentication _identityAuth;
    private readonly ICookieService _cookieService;

    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="identityAuth">Identity authentication service</param>
    /// <param name="cookieService">Cookie service</param>
    public AuthController(
        IIdentityAuthentication identityAuth,
        ICookieService cookieService)
    {
        _identityAuth = identityAuth;
        _cookieService = cookieService;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Authentication result with tokens</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Validate request
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // Register the user using the identity authentication service
        var ipAddress = GetIpAddress();
        var result = await _identityAuth.RegisterAsync(request, ipAddress);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        // Set cookies
        _cookieService.SetAuthCookies(Response, new TokenResponse
        {
            AccessToken = result.AccessToken ?? string.Empty,
            RefreshToken = result.RefreshToken ?? string.Empty,
            AccessTokenExpires = result.AccessTokenExpires ?? DateTime.UtcNow,
            RefreshTokenExpires = result.RefreshTokenExpires ?? DateTime.UtcNow
        });

        // Return response
        return Ok(result);
    }

    /// <summary>
    /// Authenticates a user
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication result with tokens</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validate request
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // Login the user using the identity authentication service
        var ipAddress = GetIpAddress();
        var result = await _identityAuth.LoginAsync(request, ipAddress);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.Message });
        }

        // Set cookies
        _cookieService.SetAuthCookies(Response, new TokenResponse
        {
            AccessToken = result.AccessToken ?? string.Empty,
            RefreshToken = result.RefreshToken ?? string.Empty,
            AccessTokenExpires = result.AccessTokenExpires ?? DateTime.UtcNow,
            RefreshTokenExpires = result.RefreshTokenExpires ?? DateTime.UtcNow
        });

        // Return response
        return Ok(result);
    }

    /// <summary>
    /// Refreshes an expired access token
    /// </summary>
    /// <param name="request">Optional refresh token request</param>
    /// <returns>New authentication tokens</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest? request = null)
    {
        // Get refresh token from cookie or request body
        var refreshToken = "";
        
        if (Request.Cookies.TryGetValue("refreshToken", out var cookieToken))
        {
            refreshToken = cookieToken;
        }
        
        // If refresh token is also in request body, it must match the cookie
        if (request?.RefreshToken != null)
        {
            if (refreshToken != "" && refreshToken != request.RefreshToken)
            {
                return Unauthorized(new { message = "Refresh token mismatch" });
            }
            
            refreshToken = request.RefreshToken;
        }
        
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token is required" });
        }

        // Refresh the token using the identity authentication service
        var ipAddress = GetIpAddress();
        var result = await _identityAuth.RefreshTokenAsync(refreshToken, ipAddress);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.Message });
        }

        // Set cookies
        _cookieService.SetAuthCookies(Response, new TokenResponse
        {
            AccessToken = result.AccessToken ?? string.Empty,
            RefreshToken = result.RefreshToken ?? string.Empty,
            AccessTokenExpires = result.AccessTokenExpires ?? DateTime.UtcNow,
            RefreshTokenExpires = result.RefreshTokenExpires ?? DateTime.UtcNow
        });

        // Return response
        return Ok(result);
    }

    /// <summary>
    /// Logs out the current user
    /// </summary>
    /// <returns>Success message</returns>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Logout()
    {
        // Get refresh token from cookie
        if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            // Revoke the token using the identity authentication service
            var ipAddress = GetIpAddress();
            await _identityAuth.LogoutAsync(refreshToken, ipAddress);
        }

        // Clear auth cookies
        _cookieService.ClearAuthCookies(Response);

        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Gets the current user's profile
    /// </summary>
    /// <returns>User profile</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        // Get user ID from claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Invalid user" });
        }

        // Get user profile using the identity authentication service
        var userProfile = await _identityAuth.GetCurrentUserAsync(userId);
        if (userProfile == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        return Ok(userProfile);
    }

    #region Helper Methods

    private string? GetIpAddress()
    {
        // Get client IP address from the request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
        }
        else
        {
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
    }

    #endregion
}