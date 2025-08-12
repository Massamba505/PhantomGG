using Microsoft.AspNetCore.Identity;
using PhantomGG.API.Common;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Managers.Interfaces;

public interface IUserManager
{
    Task<ApplicationUser> CreateUserAsync(RegisterRequest request, UserRoles role);
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);
    Task<SignInResult> ValidateCredentialsAsync(string email, string password, bool lockoutOnFailure = true);
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    UserDto MapToUserDto(ApplicationUser user);
    Task SignOutAsync();
}
