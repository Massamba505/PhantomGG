using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Managers.Interfaces;

public interface ITokenManager
{
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, string? ipAddress = null);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token, string? ipAddress = null, string? reason = null, string? replacedByToken = null);
}
