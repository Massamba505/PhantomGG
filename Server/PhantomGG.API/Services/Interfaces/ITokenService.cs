using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

/// <summary>
/// Service for handling JWT token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user
    /// </summary>
    /// <param name="user">The user to generate the token for</param>
    /// <returns>JWT token string</returns>
    string GenerateAccessToken(ApplicationUser user);
    
    /// <summary>
    /// Generates a refresh token for the specified user
    /// </summary>
    /// <param name="user">The user to generate the token for</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>Refresh token</returns>
    RefreshToken GenerateRefreshToken(ApplicationUser user, string? ipAddress = null);
    
    /// <summary>
    /// Generates both access and refresh tokens for the user
    /// </summary>
    /// <param name="user">The user to generate tokens for</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>Token response with both tokens</returns>
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, string? ipAddress = null);
    
    /// <summary>
    /// Refreshes the access token using a valid refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token string</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>New token response with refreshed tokens</returns>
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
    
    /// <summary>
    /// Revokes the specified refresh token
    /// </summary>
    /// <param name="token">Refresh token to revoke</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="replacedByToken">New token that replaces this one</param>
    /// <returns>True if successful</returns>
    Task<bool> RevokeTokenAsync(string token, string? ipAddress = null, string? reason = null, string? replacedByToken = null);
    
    /// <summary>
    /// Validates a refresh token
    /// </summary>
    /// <param name="token">Refresh token to validate</param>
    /// <returns>The refresh token if valid, null otherwise</returns>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
}