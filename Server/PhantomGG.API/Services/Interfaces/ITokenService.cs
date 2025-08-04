using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface ITokenService
{
    Task<AuthResult> GenerateAuthResponseAsync(User user);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(Guid userId, string refreshToken);
}