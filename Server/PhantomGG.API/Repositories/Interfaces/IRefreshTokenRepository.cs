using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken token);
    Task<RefreshToken?> GetByIdAsync(Guid id);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task<RefreshToken?> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(RefreshToken token);
    Task DeleteAsync(Guid id);
    Task RevokeAsync(Guid tokenId);
    Task DeleteExpiredTokensAsync();
}