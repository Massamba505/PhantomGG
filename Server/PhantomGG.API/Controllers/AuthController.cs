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
        var authTokens = await _tokenService.GenerateAuthResponseAsync(newUser);

        _cookieService.SetAuthCookies(Response, authTokens);

        return Ok(authTokens);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequestDto)
    {
        var authenticatedUser = await _authService.LoginAsync(loginRequestDto);
        var authTokens = await _tokenService.GenerateAuthResponseAsync(authenticatedUser);

        _cookieService.SetAuthCookies(Response, authTokens);

        return Ok(new AuthResponse
        {
            AccessToken = authTokens.AccessToken
        });
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var storedRefreshToken))
        {
            return Unauthorized("No refresh token found");
        }

        var refreshedTokens = await _tokenService.RefreshTokenAsync(storedRefreshToken);

        _cookieService.SetAuthCookies(Response, refreshedTokens);

        return Ok(new AuthResponse
        {
            AccessToken = refreshedTokens.AccessToken
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var storedRefreshToken))
        {
            return BadRequest("No refresh token found");
        }

        var authenticatedUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (authenticatedUserId == null)
        {
            return Unauthorized("Invalid user");
        }

        await _tokenService.RevokeRefreshTokenAsync(Guid.Parse(authenticatedUserId), storedRefreshToken);
        _cookieService.ClearAuthCookies(Response);

        return Ok(new { message = "Logged out" });
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