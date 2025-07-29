using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICookieService _cookieService;

    public AuthController(IAuthService authService, ICookieService cookieService)
    {
        _authService = authService;
        _cookieService = cookieService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterUserAsync(request);
        if (!result.Success)
            return BadRequest(new { Message = result.Message });

        return Ok(new { Message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.AuthenticateUserAsync(request.Email, request.Password);
        if (!result.Success)
            return Unauthorized(new { Message = result.Message });

        _cookieService.SetAuthCookies(Response, result.AccessToken, result.RefreshToken);
        return Ok(new { Message = "Login successful" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Invalid refresh token");

        var result = await _authService.RefreshTokensAsync(refreshToken);
        if (!result.Success)
            return Unauthorized(new { Message = result.Message });

        _cookieService.SetAuthCookies(Response, result.AccessToken, result.RefreshToken);
        return Ok(new { Message = "Tokens refreshed" });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _cookieService.ClearAuthCookies(Response);
        return Ok(new { Message = "Logout successful" });
    }
}