using PhantomGG.Models.DTOs.Auth;

namespace PhantomGG.Service.Auth.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequestDto request);
    Task<AuthDto> LoginAsync(LoginRequestDto request);
    Task<AuthDto> RefreshAsync(string refreshTokenFromCookie);
    Task LogoutAsync(string refreshTokenFromCookie);
}
