using Microsoft.AspNetCore.Identity;
using PhantomGG.API.Common;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class UserService(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUser
    ) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        if (_currentUser.UserId != userId && _currentUser.Role != UserRoles.Admin.ToString())
            throw new UnauthorizedAccessException("Access denied");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = roles.FirstOrDefault() ?? "User",
            CreatedAt = user.CreatedAt
        };
    }

    public async Task UpdateUserProfileAsync(Guid userId, UpdateUserRequest request)
    {
        if (_currentUser.UserId != userId && _currentUser.Role != UserRoles.Admin.ToString())
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null && existingUser.Id != userId)
            {
                throw new ArgumentException("Email already in use");
            }

            user.Email = request.Email;
            user.UserName = request.Email;
        }

        if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
            user.ProfilePictureUrl = request.ProfilePictureUrl;

        await _userManager.UpdateAsync(user);
    }
    
}