using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Managers.Interfaces;

public interface IRoleManager
{
    Task<bool> AddUserToRoleAsync(string userId, string roleName);
    Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
    Task<IList<string>> GetUserRolesAsync(string userId);
    Task<bool> IsUserInRoleAsync(string userId, string roleName);
    Task<bool> EnsureRoleExistsAsync(string roleName);
}
