using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.RefreshToken;

namespace PhantomGG.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync();
}
