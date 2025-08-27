using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhantomGG.API.Config;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

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
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        _cookieService.SetRefreshToken(Response, result.RefreshToken);

        var response = new ApiResponse
        {
            Success = true,
            Message = "Registered successfully",
            Data = new
            {
                user = result.User,
                accessToken = result.AccessToken,
                accessTokenExpiresAt = result.AccessTokenExpiresAt
            }
        };

        return StatusCode(201, response);
    }

    /// <summary>
    /// Authenticate user credentials and login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        _cookieService.SetRefreshToken(Response, result.RefreshToken, request.RememberMe);

        var response = new ApiResponse
        {
            Success = true,
            Message = "Login successful",
            Data = new
            {
                user = result.User,
                accessToken = result.AccessToken,
                accessTokenExpiresAt = result.AccessTokenExpiresAt
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Refresh access token using refresh token from cookie
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> Refresh()
    {
        var refreshToken = Request.Cookies[_cookieSettings.RefreshTokenCookieName];

        var result = await _authService.RefreshAsync(refreshToken ?? string.Empty);

        _cookieService.SetRefreshToken(Response, result.RefreshToken);

        var response = new ApiResponse
        {
            Success = true,
            Message = "Token refreshed successfully",
            Data = new
            {
                accessToken = result.AccessToken,
                accessTokenExpiresAt = result.AccessTokenExpiresAt
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Me()
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var result = await _userService.GetByIdAsync(currentUser.Id);

        var response = new ApiResponse
        {
            Success = true,
            Message = "User information retrieved successfully",
            Data = result
        };

        return Ok(response);
    }

    /// <summary>
    /// Logout user by clearing refresh token cookie
    /// </summary>
    [HttpPost("logout")]
    //[Authorize]
    public async Task<ActionResult<ApiResponse>> Logout()
    {
        var refreshToken = Request.Cookies[_cookieSettings.RefreshTokenCookieName];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.LogoutAsync(refreshToken);
        }

        _cookieService.ClearRefreshToken(Response);

        var response = new ApiResponse
        {
            Success = true,
            Message = "Logout successful"
        };

        return Ok(response);
    }
}
