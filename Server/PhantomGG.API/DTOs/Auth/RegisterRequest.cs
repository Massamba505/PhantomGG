using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

public class RegisterRequest
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required, StringLength(50)]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;
    public string? ProfilePicture { get; set; }
}