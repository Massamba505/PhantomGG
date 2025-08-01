using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.RefreshToken;
using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICookieService _cookieService;

    public AuthController(
        IAuthService authService,
        ICookieService cookieService)
    {
        _authService = authService;
        _cookieService = cookieService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        if (response.Success)
        {
            _cookieService.SetAuthCookies(Response, response.Tokens);
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response.Success)
        {
            _cookieService.SetAuthCookies(Response, response.Tokens);
            return Ok(response);
        }

        return Unauthorized(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);

        if (response.Success)
        {
            _cookieService.SetAuthCookies(Response, response.Tokens);
            return Ok(response);
        }

        return Unauthorized(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        _cookieService.ClearAuthCookies(Response);
        return Ok(new { Message = "Logout successful" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Message = "Invalid user information" });

        return Ok(new
        {
            UserId = userId,
            Email = email,
            Role = role
        });
    }
}
