using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Managers.Interfaces;

/// <summary>
/// Manages user roles
/// </summary>
public interface IRoleManager
{
    /// <summary>
    /// Adds a user to a role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <returns>True if successful</returns>
    Task<bool> AddUserToRoleAsync(string userId, string roleName);
    
    /// <summary>
    /// Removes a user from a role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <returns>True if successful</returns>
    Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
    
    /// <summary>
    /// Gets the roles for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of roles</returns>
    Task<IList<string>> GetUserRolesAsync(string userId);
    
    /// <summary>
    /// Checks if a user is in a role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <returns>True if user is in role</returns>
    Task<bool> IsUserInRoleAsync(string userId, string roleName);
    
    /// <summary>
    /// Ensures a role exists, creating it if needed
    /// </summary>
    /// <param name="roleName">Role name</param>
    /// <returns>True if role exists or was created</returns>
    Task<bool> EnsureRoleExistsAsync(string roleName);
}
