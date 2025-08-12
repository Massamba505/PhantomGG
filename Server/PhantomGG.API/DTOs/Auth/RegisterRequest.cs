using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

public class RegisterRequest
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
}