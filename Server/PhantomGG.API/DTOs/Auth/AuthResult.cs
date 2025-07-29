namespace PhantomGG.API.DTOs.Auth;

public class AuthResult
{
    public bool Success { get; }
    public string? Message { get; }
    public string? AccessToken { get; }
    public string? RefreshToken { get; }

    public AuthResult(bool success, string? message = null,string? accessToken = null, string? refreshToken = null)
    {
        Success = success;
        Message = message;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}