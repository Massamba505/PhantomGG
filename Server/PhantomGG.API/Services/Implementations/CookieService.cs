using PhantomGG.API.Config;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class CookieService : ICookieService
{
    private readonly JwtConfig _config;
    private readonly ILogger<CookieService> _logger;

    public CookieService(JwtConfig config, ILogger<CookieService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public void SetAuthCookies(HttpResponse response, TokenPair tokens)
    {
        response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(_config.AccessTokenExpiryMinutes),
            Path = "/",
        });

        response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_config.RefreshTokenExpiryDays),
            Path = "/",
        });
    }

    public void ClearAuthCookies(HttpResponse response)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
        };

        response.Cookies.Delete("accessToken", options);
        response.Cookies.Delete("refreshToken", options);

    }
}