using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdWithRoleAsync(Guid id);
    Task<User?> GetByEmailWithRoleAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
    Task IncrementFailedLoginAttemptsAsync(Guid userId);
    Task ResetFailedLoginAttemptsAsync(Guid userId);
    Task SetLockoutAsync(Guid userId, DateTime? lockoutEnd);
}

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> CreateAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name);
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(Guid userId);
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken> UpdateAsync(RefreshToken refreshToken);
    Task RevokeTokenAsync(string token);
    Task RevokeAllUserTokensAsync(Guid userId);
    Task DeleteExpiredTokensAsync();
}
