using Microsoft.AspNetCore.Identity;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Managers.Interfaces;

namespace PhantomGG.API.Services.Managers.Implementations;

public class UserManager(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager
    ) : IUserManager
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    public async Task<ApplicationUser> CreateUserAsync(RegisterRequest request)
    {
        var newUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ProfilePictureUrl = $"https://eu.ui-avatars.com/api/?name={request.FirstName}+{request.LastName}&size=250",
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to create user: " + 
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(newUser, "User");
        
        return newUser;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<SignInResult> ValidateCredentialsAsync(string email, string password, bool lockoutOnFailure = true)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return SignInResult.Failed;
        }
        
        return await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return null;
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        
        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = roles.FirstOrDefault() ?? "User",
            CreatedAt = user.CreatedAt
        };
    }

    public UserDto MapToUserDto(ApplicationUser user)
    {
        var roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
        
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePicture = user.ProfilePictureUrl,
            Role = roles.FirstOrDefault() ?? "User"
        };
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
