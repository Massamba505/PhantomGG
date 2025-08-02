using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(Guid userId);
    Task RevokeAsync(RefreshToken token);
}