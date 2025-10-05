using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Repository.Entities;
using System.Data;

namespace PhantomGG.Service.Mappings;

public static class AuthMappings
{
    public static User ToEntity(this RegisterRequestDto registerDto, string hashedPassword, string token)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email.ToLower(),
            PasswordHash = hashedPassword,
            ProfilePictureUrl = registerDto.ProfilePictureUrl ?? $"https://eu.ui-avatars.com/api/?name={registerDto.FirstName}+{registerDto.LastName}&size=250",
            Role = !string.IsNullOrEmpty(registerDto.Role) ? registerDto.Role : "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            EmailVerified = false,
            EmailVerificationToken = token,
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1),
            FailedLoginAttempts = 0
        };
    }
}