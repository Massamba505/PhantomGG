using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

/// <summary>
/// Repository for managing users
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>User if found, null otherwise</returns>
    Task<ApplicationUser?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User if found, null otherwise</returns>
    Task<ApplicationUser?> GetByIdAsync(string id);
    
    /// <summary>
    /// Adds a new user
    /// </summary>
    /// <param name="user">User to add</param>
    Task AddAsync(ApplicationUser user);
    
    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="user">User to update</param>
    Task UpdateAsync(ApplicationUser user);
    
    /// <summary>
    /// Checks if an email address is already in use
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>True if the email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(string email);
}