using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface IRefreshTokenService
{
    Task CreateRefreshTokenAsync(Guid userId, string refreshToken);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(Guid tokenId);
}
