using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.DTOs.Auth;

public class AuthDto
{
    public string AccessToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}
