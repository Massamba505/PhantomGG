using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(Guid userId);
}