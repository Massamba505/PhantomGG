using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.User;

public class CurrentUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRoles Role { get; set; }
}
