using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.User;

public class UpdateUserRequest
{
    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? ProfilePictureUrl { get; set; }
}
