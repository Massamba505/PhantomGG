using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.Auth.Requests;
using PhantomGG.API.DTOs.Auth.Responses;

namespace PhantomGG.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RefreshTokenAsync();
    Task<LogoutResponse> RevokeTokenAsync();
    Task<LogoutResponse> RevokeAllUserTokensAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
    Task<AuthResponse> GetCurrentUserAsync();
}
