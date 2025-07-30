using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.DTOs.Auth;

public class AuthResult
{
    public bool Success { get; }
    public string Message { get; }
    public string AccessToken { get; }
    public string RefreshToken { get; }

    public AuthResult(bool success, string message = "",string accessToken = "", string refreshToken = "")
    {
        Success = success;
        Message = message;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public UserProfileDto User { get; set; } = null!;
    public TokenPair Tokens { get; set; } = null!;
}