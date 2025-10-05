using PhantomGG.Repository.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailVerificationTokenAsync(string token);
    Task<User?> GetByPasswordResetTokenAsync(string token);
    Task<bool> EmailExistsAsync(string email);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
}
