using PhantomGG.Models.DTOs.Auth;

namespace PhantomGG.Service.Interfaces;

public interface IAuthService
{
    Task<AuthDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthDto> LoginAsync(LoginRequestDto request);
    Task<AuthDto> RefreshAsync(string refreshTokenFromCookie);
    Task LogoutAsync(string refreshTokenFromCookie);
}
