using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface ITokenService
{
    Task<TokenPair> GenerateAuthResponseAsync(User user);
    Task<TokenPair> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(Guid userId, string refreshToken);
}