using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdWithRoleAsync(Guid id);
    Task<User?> GetByEmailWithRoleAsync(string email);
    Task<User?> GetByNameAsync(string firstName, string lastName);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> NameExistsAsync(string firstName, string lastName);
    Task IncrementFailedLoginAttemptsAsync(Guid userId);
    Task ResetFailedLoginAttemptsAsync(Guid userId);
    Task SetLockoutAsync(Guid userId, DateTime? lockoutEnd);
}
