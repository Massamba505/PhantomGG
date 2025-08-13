using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(AspNetUser user);
    RefreshToken GenerateRefreshToken(AspNetUser user);
    Task<TokenResponse> GenerateTokensAsync(AspNetUser user);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string token);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
}