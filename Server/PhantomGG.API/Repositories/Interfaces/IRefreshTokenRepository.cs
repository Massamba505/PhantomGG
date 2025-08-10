using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

/// <summary>
/// Repository for managing refresh tokens
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Adds a refresh token
    /// </summary>
    /// <param name="token">The token to add</param>
    Task AddAsync(RefreshToken token);
    
    /// <summary>
    /// Gets a refresh token by its token value
    /// </summary>
    /// <param name="token">The token value</param>
    /// <returns>The refresh token if found and valid, null otherwise</returns>
    Task<RefreshToken?> GetByTokenAsync(string token);
    
    /// <summary>
    /// Gets all valid refresh tokens for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of valid refresh tokens</returns>
    Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(string userId);
}