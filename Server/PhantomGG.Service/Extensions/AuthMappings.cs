using PhantomGG.Models.DTOs.Auth;
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
            Email = registerDto.Email.ToLower(),
            PasswordHash = hashedPassword,
            ProfilePictureUrl = registerDto.ProfilePictureUrl ?? $"https://eu.ui-avatars.com/api/?name={registerDto.FirstName}+{registerDto.LastName}&size=250",
            Role = (int)registerDto.Role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            EmailVerified = false,
            EmailVerificationToken = null,
            EmailVerificationTokenExpiry = null,
            FailedLoginAttempts = 0
        };
    }
}