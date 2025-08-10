using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// User's first name
    /// </summary>
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to user's profile picture
    /// </summary>
    public string? ProfilePicture { get; set; }
}