using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

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

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse>> GetUserStats()
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var stats = await _userService.GetUserStatsAsync(currentUser.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "User stats retrieved successfully",
            Data = stats
        });
    }

    [HttpGet("activity")]
    public async Task<ActionResult<ApiResponse>> GetUserActivity([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var activity = await _userService.GetUserActivityAsync(currentUser.Id, page, limit);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "User activity retrieved successfully",
            Data = activity
        });
    }

    [HttpPost("profile-picture")]
    public async Task<ActionResult<ApiResponse>> UploadProfilePicture(IFormFile profilePicture)
    {
        if (profilePicture == null || profilePicture.Length == 0)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "No file provided"
            });
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
