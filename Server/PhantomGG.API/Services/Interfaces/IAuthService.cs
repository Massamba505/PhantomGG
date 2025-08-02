using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

public interface IAuthService
{
    Task<TokenPair> RegisterAsync(RegisterRequest request);
    Task<TokenPair> LoginAsync(LoginRequest request);
    Task<TokenPair> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(Guid userId, string refreshToken);
}