using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

public class RegisterRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [StringLength(255, MinimumLength = 1)]
    public string? ProfilePictureUrl { get; set; }

    public string? Role { get; set; }
}
