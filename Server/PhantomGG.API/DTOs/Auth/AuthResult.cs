namespace PhantomGG.API.DTOs.Auth;

public class AuthResult
{
    public string AccessToken { get; }
    public string? RefreshToken { get; }

    public AuthResult(string accessToken = "", string? refreshToken = "")
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
}