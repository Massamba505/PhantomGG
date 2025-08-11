using Microsoft.AspNetCore.Identity;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Managers.Interfaces;

namespace PhantomGG.API.Services.Managers.Implementations;

public class RoleManager(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager
    ) : IRoleManager
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;

    public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        await EnsureRoleExistsAsync(roleName);
        
        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

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

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new List<string>();
        }
        
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<bool> EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            return result.Succeeded;
        }
        
        return true;
    }
}
