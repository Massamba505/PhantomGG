namespace PhantomGG.API.Config;

/// <summary>
/// Configuration for JWT authentication
/// </summary>
public class JwtConfig
{
    /// <summary>
    /// Secret key used to sign the JWT token
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Issuer of the JWT token
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audience of the JWT token
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Expiry time for access tokens in minutes
    /// </summary>
    public int AccessTokenExpiryMinutes { get; set; } = 15;

    /// <summary>
    /// Expiry time for refresh tokens in days
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
