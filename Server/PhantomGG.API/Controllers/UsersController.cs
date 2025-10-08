using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Models.DTOs.User;
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
    [Authorize]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var currentUser = _currentUserService.GetCurrentUser()!;
        var profile = await _userService.GetByIdAsync(currentUser.Id);
        return Ok(profile);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    [HttpPatch("profile")]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        var currentUser = _currentUserService.GetCurrentUser()!;
        var updatedUser = await _userService.UpdateProfileAsync(currentUser.Id, request);
        return Ok(updatedUser);
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPatch("password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var currentUser = _currentUserService.GetCurrentUser()!;
        await _userService.ChangePasswordAsync(currentUser.Id, request);
        return NoContent();
    }

    /// <summary>
    /// Upload profile picture
    /// </summary>
    [HttpPost("profile-picture")]
    public async Task<ActionResult<ProfilePictureUploadDto>> UploadProfilePicture(IFormFile profilePicture)
    {
        if (profilePicture == null || profilePicture.Length == 0)
        {
            return BadRequest("No file provided");
        }

        var currentUser = _currentUserService.GetCurrentUser()!;
        var result = await _userService.UploadProfilePictureAsync(currentUser.Id, profilePicture);
        return Ok(result);
    }
}
