using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Managers.Interfaces;

public interface IRoleManager
{
    Task<bool> AddUserToRoleAsync(Guid userId, string roleName);
    Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName);
    Task<IList<string>> GetUserRolesAsync(Guid userId);
    Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
    Task<bool> EnsureRoleExistsAsync(string roleName);
}
