using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs.Auth.Requests;
using PhantomGG.API.DTOs.Auth.Responses;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> RefreshToken()
    {
        var response = await _authService.RefreshTokenAsync();
        return Ok(response);
    }

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    [HttpPost("revoke")]
    public async Task<ActionResult<LogoutResponse>> RevokeToken()
    {
        var response = await _authService.RevokeTokenAsync();
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Revoke all refresh tokens for the current user
    /// </summary>
    [HttpPost("revoke-all")]
    [Authorize]
    public async Task<ActionResult<LogoutResponse>> RevokeAllTokens()
    {
        if (CurrentUserId == null)
        {
            throw new UnauthorizedAccessException("Invalid user");
        }

        var response = await _authService.RevokeAllUserTokensAsync(CurrentUserId.Value);
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> GetCurrentUser()
    {
        var response = await _authService.GetCurrentUserAsync();
        return Ok(response);
    }

    /// <summary>
    /// Cleanup expired refresh tokens (admin only)
    /// </summary>
    [HttpPost("cleanup")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CleanupExpiredTokens()
    {
        await _authService.CleanupExpiredTokensAsync();
        return Ok(new { message = "Expired tokens cleaned up successfully" });
    }
}
