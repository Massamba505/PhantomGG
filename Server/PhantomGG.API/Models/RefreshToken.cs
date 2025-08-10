using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhantomGG.API.Models;

/// <summary>
/// Represents a refresh token for JWT authentication
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Primary key for the refresh token
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The refresh token value
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the refresh token expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Date and time when the refresh token was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the refresh token was revoked, if applicable
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// IP address where the token was created
    /// </summary>
    [MaxLength(50)]
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// IP address where the token was revoked, if applicable
    /// </summary>
    [MaxLength(50)]
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Token that replaced this refresh token
    /// </summary>
    [MaxLength(255)]
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Reason for token revocation, if applicable
    /// </summary>
    [MaxLength(255)]
    public string? ReasonRevoked { get; set; }

    /// <summary>
    /// Foreign key to the user who owns this refresh token
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the associated user
    /// </summary>
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Whether the refresh token is active
    /// </summary>
    public bool IsActive => RevokedAt == null && !IsExpired;

    /// <summary>
    /// Whether the refresh token has expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
