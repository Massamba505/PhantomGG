using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetByIdAsync(Guid id);
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken> UpdateAsync(RefreshToken refreshToken);
    Task DeleteAsync(Guid id);
    Task RevokeAsync(string token);
    Task RevokeAllUserTokensAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
}
