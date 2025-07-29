using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    public string? ProfilePicture { get; set; }
}
