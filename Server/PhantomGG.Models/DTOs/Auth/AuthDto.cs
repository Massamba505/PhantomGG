using PhantomGG.Models.DTOs.User;

namespace PhantomGG.Models.DTOs.Auth;

public class AuthDto
{
    public string AccessToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public record RefreshTokenResponse(string accessToken);