using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(
    ICurrentUserService currentUserService,
    IUserService userService) : ControllerBase
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse>> GetProfile()
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var profile = await _userService.GetByIdAsync(currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = profile,
            Message = "Profile retrieved successfully"
        });
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="request">Profile update data</param>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse>> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var updatedUser = await _userService.UpdateProfileAsync(currentUser.Id, request);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Profile updated successfully",
            Data = updatedUser
        });
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="request">Password change data including current and new password</param>
    [HttpPut("change-password")]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _userService.ChangePasswordAsync(currentUser.Id, request);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Password updated successfully"
        });
    }

    /// <summary>
    /// Upload profile picture
    /// </summary>
    /// <param name="profilePicture">The profile picture image file</param>
    [HttpPost("profile-picture")]
    public async Task<ActionResult<ApiResponse>> UploadProfilePicture(IFormFile profilePicture)
    {
        if (profilePicture == null || profilePicture.Length == 0)
        {
            throw new ValidationException("No file provided");
        }

        var currentUser = _currentUserService.GetCurrentUser();
        var result = await _userService.UploadProfilePictureAsync(currentUser.Id, profilePicture);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Profile picture uploaded successfully",
            Data = result
        });
    }
}
