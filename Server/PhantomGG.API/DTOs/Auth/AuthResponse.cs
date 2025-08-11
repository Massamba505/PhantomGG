using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.DTOs.Auth;

public class AuthResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpires { get; set; }
    public DateTime? RefreshTokenExpires { get; set; }
    public UserDto? User { get; set; }
}