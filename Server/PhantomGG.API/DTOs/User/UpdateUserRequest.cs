using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.User;

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Role { get; set; }
}
