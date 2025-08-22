using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.DTOs.Auth;

public class AuthDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}
