using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New token pair</returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    /// <param name="request">Token to revoke</param>
    /// <returns>Success status</returns>
    [HttpPost("revoke")]
    [Authorize]
    public async Task<ActionResult> RevokeToken([FromBody] RefreshRequest request)
    {
        try
        {
            var success = await _authService.RevokeTokenAsync(request.RefreshToken);
            if (success)
            {
                return Ok(new { message = "Token revoked successfully" });
            }
            return BadRequest(new { message = "Failed to revoke token" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred during token revocation" });
        }
    }

    /// <summary>
    /// Revoke all refresh tokens for the current user
    /// </summary>
    /// <returns>Success status</returns>
    [HttpPost("revoke-all")]
    [Authorize]
    public async Task<ActionResult> RevokeAllTokens()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var success = await _authService.RevokeAllUserTokensAsync(userId);
            if (success)
            {
                return Ok(new { message = "All tokens revoked successfully" });
            }
            return BadRequest(new { message = "Failed to revoke tokens" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred during token revocation" });
        }
    }

    /// <summary>
    /// Get current user information (test endpoint for authentication)
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [Authorize]
    public ActionResult GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            return Ok(new
            {
                id = userIdClaim?.Value,
                email = emailClaim?.Value,
                role = roleClaim?.Value,
                isAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred retrieving user information" });
        }
    }

    /// <summary>
    /// Cleanup expired refresh tokens (admin only)
    /// </summary>
    /// <returns>Success status</returns>
    [HttpPost("cleanup")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CleanupExpiredTokens()
    {
        try
        {
            await _authService.CleanupExpiredTokensAsync();
            return Ok(new { message = "Expired tokens cleaned up successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred during cleanup" });
        }
    }
}
