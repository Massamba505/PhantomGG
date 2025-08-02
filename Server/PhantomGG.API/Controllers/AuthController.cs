using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhantomGG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICookieService _cookieService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, ICookieService cookieService, IUserService userService)
    {
        _authService = authService;
        _cookieService = cookieService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var authResponse = await _authService.RegisterAsync(request);

        _cookieService.SetAuthCookies(Response, authResponse);

        return Ok(new AuthResponse
        {
            AccessToken = authResponse.AccessToken
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var authResponse = await _authService.LoginAsync(request);

        _cookieService.SetAuthCookies(Response, authResponse);

        return Ok(new AuthResponse
        {
            AccessToken = authResponse.AccessToken
        });
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized("No refresh token found");
        }

        var authResponse = await _authService.RefreshTokenAsync(refreshToken);

        _cookieService.SetAuthCookies(Response, authResponse);

        return Ok(new AuthResponse
        {
            AccessToken = authResponse.AccessToken
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return BadRequest("No refresh token found");
        }

        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        { 
            return Unauthorized("Invalid user"); 
        }

        await _authService.RevokeRefreshTokenAsync(Guid.Parse(userId), refreshToken);
        _cookieService.ClearAuthCookies(Response);

        return Ok(new { message = "Logged out" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized("User not found");
        }

        var result = await _userService.GetUserProfileAsync(user.Id);

        return Ok(result);
    }
}