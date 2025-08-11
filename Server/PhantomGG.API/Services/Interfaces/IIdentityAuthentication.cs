using Microsoft.AspNetCore.Identity;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

/// <summary>
/// Comprehensive identity and authentication service interface
/// </summary>
public interface IIdentityAuthentication
{
    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ipAddress = null);
    
    /// <summary>
    /// Authenticates a user with username and password
    /// </summary>
    /// <param name="request">Login request</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress = null);
    
    /// <summary>
    /// Refreshes the authentication tokens
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>Authentication response with new tokens</returns>
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
    
    /// <summary>
    /// Logs out the current user
    /// </summary>
    /// <param name="refreshToken">Current refresh token to revoke</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>True if successful</returns>
    Task<bool> LogoutAsync(string refreshToken, string? ipAddress = null);
    
    /// <summary>
    /// Gets the current authenticated user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User profile</returns>
    Task<UserProfileDto?> GetCurrentUserAsync(Guid userId);
    
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
    /// Adds a user to a role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <returns>True if successful</returns>
    Task<bool> AddUserToRoleAsync(Guid userId, string roleName);
    
    /// <summary>
    /// Removes a user from a role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <returns>True if successful</returns>
    Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName);
    
    /// <summary>
    /// Gets the roles for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of roles</returns>
    Task<IList<string>> GetUserRolesAsync(Guid userId);
    
    /// <summary>
    /// Checks if a user is in a role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <returns>True if user is in role</returns>
    Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
    
    /// <summary>
    /// Generates and returns access and refresh tokens
    /// </summary>
    /// <param name="user">Application user</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>Token response</returns>
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, string? ipAddress = null);
    
    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="replacedByToken">Token that replaced this one</param>
    /// <returns>True if successful</returns>
    Task<bool> RevokeTokenAsync(string token, string? ipAddress = null, string? reason = null, string? replacedByToken = null);
    
    /// <summary>
    /// Validates a refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <returns>The refresh token if valid, null otherwise</returns>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
}
