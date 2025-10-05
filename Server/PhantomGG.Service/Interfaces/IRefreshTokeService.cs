using PhantomGG.Models.DTOs.AuthToken;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task<RefreshTokenDto> AddRefreshToken(User user);
    Task<RefreshToken> DeleteAsync(string refreshTokenFromCookie);
    Task DeleteAllForUserAsync(Guid userId);
}
