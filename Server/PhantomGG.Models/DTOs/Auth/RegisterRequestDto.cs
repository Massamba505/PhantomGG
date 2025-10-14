using PhantomGG.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Auth;

public class RegisterRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public UserRoles Role { get; set; }
}
