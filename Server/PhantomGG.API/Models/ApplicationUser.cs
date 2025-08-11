using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.Models;

/// <summary>
/// Extending the default IdentityUser to include additional user profile information
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    [MaxLength(255)]
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
