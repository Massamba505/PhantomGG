using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task CreateAsync(RefreshToken token);
    Task DeleteAsync(RefreshToken token);
    Task DeleteAllForUserAsync(Guid userId);
}
