using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Mappings;

public static class AuthMappings
{
    public static User ToEntity(this RegisterRequestDto registerDto, string hashedPassword)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            PasswordHash = hashedPassword,
            Role = !string.IsNullOrEmpty(registerDto.Role) ? registerDto.Role : "User",
            ProfilePictureUrl = registerDto.ProfilePictureUrl ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}