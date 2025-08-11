using Microsoft.AspNetCore.Identity;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Managers.Interfaces;

namespace PhantomGG.API.Services.Managers.Implementations;

/// <summary>
/// Implementation of the role manager
/// </summary>
public class RoleManager : IRoleManager
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    /// <summary>
    /// Initializes a new instance of the RoleManager
    /// </summary>
    /// <param name="userManager">Identity user manager</param>
    /// <param name="roleManager">Identity role manager</param>
    public RoleManager(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <inheritdoc />
    public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        // Ensure role exists
        await EnsureRoleExistsAsync(roleName);
        
        // Add user to role
        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }

    /// <inheritdoc />
    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new List<string>();
        }
        
        return await _userManager.GetRolesAsync(user);
    }

    /// <inheritdoc />
    public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    /// <inheritdoc />
    public async Task<bool> EnsureRoleExistsAsync(string roleName)
    {
        // Check if role exists
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            // Create role if it doesn't exist
            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            return result.Succeeded;
        }
        
        return true;
    }
}
