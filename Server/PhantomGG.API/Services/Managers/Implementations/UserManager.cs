using Microsoft.AspNetCore.Identity;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Managers.Interfaces;

namespace PhantomGG.API.Services.Managers.Implementations;

/// <summary>
/// Implementation of the user manager
/// </summary>
public class UserManager : IUserManager
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    /// <summary>
    /// Initializes a new instance of the UserManager
    /// </summary>
    /// <param name="userManager">Identity user manager</param>
    /// <param name="signInManager">Identity sign-in manager</param>
    public UserManager(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    /// <inheritdoc />
    public async Task<ApplicationUser> CreateUserAsync(RegisterRequest request)
    {
        // Create new user
        var newUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ProfilePictureUrl = $"https://eu.ui-avatars.com/api/?name={request.FirstName}+{request.LastName}&size=250",
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true // For simplicity, auto-confirm email
        };

        // Create user with password
        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to create user: " + 
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Assign default role
        await _userManager.AddToRoleAsync(newUser, "User");
        
        return newUser;
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    /// <inheritdoc />
    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    /// <inheritdoc />
    public async Task<SignInResult> ValidateCredentialsAsync(string email, string password, bool lockoutOnFailure = true)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return SignInResult.Failed;
        }
        
        return await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
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

    /// <inheritdoc />
    public UserDto MapToUserDto(ApplicationUser user)
    {
        // Get user roles asynchronously and wait for the result
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

    /// <inheritdoc />
    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
