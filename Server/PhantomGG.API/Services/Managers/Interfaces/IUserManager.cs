using Microsoft.AspNetCore.Identity;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Managers.Interfaces;

/// <summary>
/// Manages user-related operations
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <returns>The created user</returns>
    Task<ApplicationUser> CreateUserAsync(RegisterRequest request);
    
    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Application user</returns>
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    
    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>Application user</returns>
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    
    /// <summary>
    /// Checks if a user exists with the given email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>True if user exists</returns>
    Task<bool> UserExistsAsync(string email);
    
    /// <summary>
    /// Validates user credentials
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="lockoutOnFailure">Whether to lock out on failure</param>
    /// <returns>SignInResult indicating success or failure</returns>
    Task<SignInResult> ValidateCredentialsAsync(string email, string password, bool lockoutOnFailure = true);
    
    /// <summary>
    /// Gets a user profile by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User profile DTO</returns>
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    
    /// <summary>
    /// Maps an ApplicationUser to a UserDto
    /// </summary>
    /// <param name="user">Application user</param>
    /// <returns>User DTO</returns>
    UserDto MapToUserDto(ApplicationUser user);
    
    /// <summary>
    /// Signs out the current user
    /// </summary>
    Task SignOutAsync();
}
