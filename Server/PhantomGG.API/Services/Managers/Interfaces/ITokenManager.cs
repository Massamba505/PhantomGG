using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Managers.Interfaces;

public interface ITokenManager
{
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
}
