using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<string> CreateRefreshTokenAsync(Guid userId);
    Task RevokeTokenAsync(Guid tokenId);
    Task RevokeAllTokensForUserAsync(Guid userId);
    Task<RefreshToken> ValidateTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetTokensForUserAsync(Guid userId);
    Task DeleteExpiredTokensAsync();
    Task<string> RotateTokenAsync(string oldToken);
    Task<bool> IsTokenExpiringSoonAsync(Guid tokenId);
}
