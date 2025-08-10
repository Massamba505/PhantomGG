using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.DTOs.Auth;

/// <summary>
/// Internal result object for authentication operations
/// </summary>
public class AuthResult
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Expiration time of the access token
    /// </summary>
    public DateTime AccessTokenExpires { get; set; }
    
    /// <summary>
    /// Expiration time of the refresh token
    /// </summary>
    public DateTime RefreshTokenExpires { get; set; }
}

/// <summary>
/// Response object for authentication endpoints
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Whether the authentication operation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Optional message
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// JWT access token
    /// </summary>
    public string? AccessToken { get; set; }
    
    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Expiration time of the access token
    /// </summary>
    public DateTime? AccessTokenExpires { get; set; }
    
    /// <summary>
    /// Expiration time of the refresh token
    /// </summary>
    public DateTime? RefreshTokenExpires { get; set; }
    
    /// <summary>
    /// User information
    /// </summary>
    public UserDto? User { get; set; }
}