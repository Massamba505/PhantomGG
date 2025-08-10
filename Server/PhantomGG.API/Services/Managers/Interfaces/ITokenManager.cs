using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Managers.Interfaces;

/// <summary>
/// Manages token generation, validation, and revocation
/// </summary>
public interface ITokenManager
{
    /// <summary>
    /// Generates access and refresh tokens for a user
    /// </summary>
    /// <param name="user">Application user</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>Token response with access and refresh tokens</returns>
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, string? ipAddress = null);
    
    /// <summary>
    /// Validates a refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <returns>The refresh token if valid, null otherwise</returns>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
    
    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="replacedByToken">Token that replaced this one</param>
    /// <returns>True if successful</returns>
    Task<bool> RevokeTokenAsync(string token, string? ipAddress = null, string? reason = null, string? replacedByToken = null);
}
