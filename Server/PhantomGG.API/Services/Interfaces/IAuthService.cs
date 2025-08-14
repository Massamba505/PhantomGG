using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RefreshTokenAsync(RefreshRequest request);
    Task<bool> RevokeTokenAsync(string token);
    Task<bool> RevokeAllUserTokensAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
}
