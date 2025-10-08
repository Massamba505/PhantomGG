using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

[Route("api/auth")]
[ApiController]
[AllowAnonymous]
[EnableRateLimiting("AuthVerificationPolicy")]
public class AuthVerificationController(IAuthVerificationService authVerificationService) : ControllerBase
{
    private readonly IAuthVerificationService _authVerificationService = authVerificationService;

    /// <summary>
    /// Confirm user's email using a verification token
    /// </summary>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        var success = await _authVerificationService.VerifyEmailAsync(request.Token);

        if (!success)
        {
            return BadRequest("Invalid or expired verification token");
        }

        return Ok("Email verified successfully");
    }

    /// <summary>
    /// Send a new email verification link
    /// </summary>
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
    {
        await _authVerificationService.ResendEmailVerificationAsync(request.Email);

        return Ok("Verification email sent");
    }

    /// <summary>
    /// Send password reset link to user's email
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authVerificationService.InitiatePasswordResetAsync(request.Email);

        return Ok("If an account exists with that email, a password reset link has been sent");
    }

    /// <summary>
    /// Reset password using reset token and new password
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var success = await _authVerificationService.ResetPasswordAsync(request.Token, request.NewPassword);

        if (!success)
        {
            return BadRequest("Invalid or expired reset token");
        }

        return Ok("Password reset successfully");
    }
}