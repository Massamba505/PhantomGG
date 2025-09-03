using PhantomGG.API.DTOs.AuthToken;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface IRefreshTokeService
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task<RefreshTokenDto> AddRefreshToken(User user);
    Task<RefreshToken> DeleteAsync(string refreshTokenFromCookie);
    Task DeleteAllForUserAsync(Guid userId);
}
