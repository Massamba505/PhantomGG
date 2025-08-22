using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthDto> LoginAsync(LoginRequestDto request);
    Task<AuthDto> RefreshAsync(string refreshTokenFromCookie);
    Task LogoutAsync(string refreshTokenFromCookie);
}
