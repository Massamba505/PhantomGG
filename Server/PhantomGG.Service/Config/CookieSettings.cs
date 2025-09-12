namespace PhantomGG.Service.Config;

public class CookieSettings
{
    public string RefreshTokenCookieName { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string Path { get; set; } = "/";
    public bool Secure { get; set; }
    public bool HttpOnly { get; set; }
}
