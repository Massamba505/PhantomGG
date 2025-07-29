using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterUserAsync(RegisterRequest request);
    Task<AuthResult> AuthenticateUserAsync(string email, string password);
    Task<AuthResult> RefreshTokensAsync(string refreshToken);
}
