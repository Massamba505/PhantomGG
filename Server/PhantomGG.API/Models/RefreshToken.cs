using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhantomGG.API.Models;

/// <summary>
/// Represents a refresh token for JWT authentication
/// </summary>
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    [MaxLength(50)]
    public string? CreatedByIp { get; set; }

    [MaxLength(50)]
    public string? RevokedByIp { get; set; }

    [MaxLength(255)]
    public string? ReplacedByToken { get; set; }

    [MaxLength(255)]
    public string? ReasonRevoked { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
    
    public bool IsActive => RevokedAt == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
