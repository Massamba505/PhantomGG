using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICookieService _cookieService;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, ICookieService cookieService, IUserService userService, ITokenService tokenService)
    {
        _authService = authService;
        _cookieService = cookieService;
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequestDto)
    {
        var newUser = await _authService.RegisterAsync(registerRequestDto);
        var authResult = await _tokenService.GenerateAuthResponseAsync(newUser);

        _cookieService.SetAuthCookies(Response, authResult);

        return Ok(new AuthResponse
        {
            AccessToken = authResult.AccessToken,
            RefreshToken = authResult.RefreshToken,
            AccessTokenExpires = authResult.AccessTokenExpires,
            RefreshTokenExpires = authResult.RefreshTokenExpires
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequestDto)
    {
        var authenticatedUser = await _authService.LoginAsync(loginRequestDto);
        var authResult = await _tokenService.GenerateAuthResponseAsync(authenticatedUser);

        _cookieService.SetAuthCookies(Response, authResult);

        return Ok(new AuthResponse
        {
            AccessToken = authResult.AccessToken,
            RefreshToken = authResult.RefreshToken,
            AccessTokenExpires = authResult.AccessTokenExpires,
            RefreshTokenExpires = authResult.RefreshTokenExpires
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest? request = null)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Refresh token is required" });
        }

        if (!Request.Cookies.TryGetValue("refreshToken", out var cookieRefreshToken))
        {
            return Unauthorized(new { message = "No refresh token found in cookies" });
        }

        if (request?.RefreshToken != cookieRefreshToken)
        {
            return Unauthorized(new { message = "Refresh token mismatch" });
        }

        var refreshedResult = await _tokenService.RefreshTokenAsync(cookieRefreshToken);

        _cookieService.SetAuthCookies(Response, refreshedResult);

        return Ok(new AuthResponse
        {
            AccessToken = refreshedResult.AccessToken,
            RefreshToken = refreshedResult.RefreshToken,
            AccessTokenExpires = refreshedResult.AccessTokenExpires,
            RefreshTokenExpires = refreshedResult.RefreshTokenExpires
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        _cookieService.ClearAuthCookies(Response);

        if (Request.Cookies.TryGetValue("refreshToken", out var storedRefreshToken))
        {
            var authenticatedUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (authenticatedUserId != null)
            {
                await _tokenService.RevokeRefreshTokenAsync(Guid.Parse(authenticatedUserId), storedRefreshToken);
            }
        }

        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var currentUser = HttpContext.Items["User"] as User;
        if (currentUser == null)
        {
            return Unauthorized("User not found");
        }

        var userProfile = await _userService.GetUserProfileAsync(currentUser.Id);

        return Ok(userProfile);
    }
}