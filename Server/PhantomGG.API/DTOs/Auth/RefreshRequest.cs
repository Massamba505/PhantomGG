using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

/// <summary>
/// Request model for token refresh
/// </summary>
public class RefreshRequest
{
    /// <summary>
    /// Refresh token
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the refresh token should be persisted
    /// </summary>
    public bool PersistCookie { get; set; } = true;
}
