using PhantomGG.Models.DTOs.User;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this User user)
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

    public static UserDto ToOrganizerDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public static void UpdateEntity(this UpdateUserProfileRequest updateDto, User existingUser)
    {
        if (!string.IsNullOrEmpty(updateDto.FirstName))
            existingUser.FirstName = updateDto.FirstName;
        if (!string.IsNullOrEmpty(updateDto.LastName))
            existingUser.LastName = updateDto.LastName;
        if (!string.IsNullOrEmpty(updateDto.Email))
            existingUser.Email = updateDto.Email;
    }
}