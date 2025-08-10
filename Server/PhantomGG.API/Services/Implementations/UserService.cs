using Microsoft.AspNetCore.Identity;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

/// <summary>
/// Implementation of the user service
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    /// <summary>
    /// Initializes a new instance of the UserService
    /// </summary>
    /// <param name="userManager">User manager</param>
    /// <param name="currentUser">Current user service</param>
    public UserService(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<UserProfileDto> GetUserProfileAsync(string userId)
    {
        if (_currentUser.UserId != userId && _currentUser.Role != "Admin")
            throw new UnauthorizedAccessException("Access denied");

        var user = await _userManager.FindByIdAsync(userId);
        if(user == null)
            throw new KeyNotFoundException("User not found");
        
        // Get user roles
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

    /// <inheritdoc />
    public async Task UpdateUserProfileAsync(string userId, UpdateUserRequest request)
    {
        if (_currentUser.UserId != userId && _currentUser.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        var user = await _userManager.FindByIdAsync(userId);
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
            user.UserName = request.Email; // Username is the same as email
        }

        if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
            user.ProfilePictureUrl = request.ProfilePictureUrl;

        await _userManager.UpdateAsync(user);
    }
}