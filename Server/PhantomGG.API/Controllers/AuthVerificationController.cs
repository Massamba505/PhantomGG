using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

[Route("api/auth")]
[ApiController]
[AllowAnonymous]
public class AuthVerificationController(IAuthVerificationService authVerificationService) : ControllerBase
{
    private readonly IAuthVerificationService _authVerificationService = authVerificationService;

    [HttpPost("verify-email")]
    public async Task<ActionResult<ApiResponse>> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        var success = await _authVerificationService.VerifyEmailAsync(request.Token);

        if (!success)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid or expired verification token"
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Email verified successfully"
        });
    }

    [HttpPost("resend-verification")]
    public async Task<ActionResult<ApiResponse>> ResendVerification([FromBody] ResendVerificationRequest request)
    {
        await _authVerificationService.ResendEmailVerificationAsync(request.Email);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Verification email sent"
        });
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authVerificationService.InitiatePasswordResetAsync(request.Email);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "If an account exists with that email, a password reset link has been sent"
        });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var success = await _authVerificationService.ResetPasswordAsync(request.Token, request.NewPassword);

        if (!success)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid or expired reset token"
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Password reset successfully"
        });
    }
}