using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task UpdateUserAsync(Guid userId, UpdateUserRequest request);
    Task DeleteUserAsync(Guid userId);
    Task UpdateUserRoleAsync(Guid userId, string newRole);
}
