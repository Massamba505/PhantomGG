using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.Models;

/// <summary>
/// Extends the default IdentityUser to include additional user profile information
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User's first name
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// URL to the user's profile picture
    /// </summary>
    [MaxLength(255)]
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Date and time when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property for refresh tokens associated with this user
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
