using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.Services.Interfaces;

/// <summary>
/// Service for managing user profiles
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets a user's profile
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User profile</returns>
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    
    /// <summary>
    /// Updates a user's profile
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Update request</param>
    Task UpdateUserProfileAsync(Guid userId, UpdateUserRequest request);
}
