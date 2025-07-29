using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<IEnumerable<RefreshToken>> GetAllAsync();
    Task<RefreshToken?> GetByIdAsync(Guid tokenId);
    Task<IEnumerable<RefreshToken>> GetTokensByUserIdAsync(Guid userId);
    Task<RefreshToken?> GetValidTokenAsync(string token);
    Task CreateAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task DeleteAsync(Guid tokenId);
    Task RevokeAsync(Guid tokenId);
    Task DeleteExpiredTokensAsync();
}
