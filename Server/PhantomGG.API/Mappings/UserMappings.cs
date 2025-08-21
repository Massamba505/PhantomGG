using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;

namespace PhantomGG.API.Mappings;

public static class UserMappings
{
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }
}
