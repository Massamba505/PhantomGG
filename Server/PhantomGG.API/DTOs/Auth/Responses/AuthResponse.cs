using PhantomGG.API.DTOs.User.Responses;

namespace PhantomGG.API.DTOs.Auth.Responses;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}
