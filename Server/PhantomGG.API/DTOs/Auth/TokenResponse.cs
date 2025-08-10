namespace PhantomGG.API.DTOs.Auth;

/// <summary>
/// Response object containing authentication tokens
/// </summary>
public class TokenResponse
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
