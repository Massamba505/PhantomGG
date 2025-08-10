using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}