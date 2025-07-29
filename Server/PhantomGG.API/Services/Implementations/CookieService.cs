using Microsoft.Extensions.Options;
using PhantomGG.API.Config;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class CookieService : ICookieService
{
    private readonly JwtConfig _jwtConfig;

    public CookieService(IOptions<JwtConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
    }

    public void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken)
    {
        SetCookie(response, "accessToken", accessToken, _jwtConfig.AccessTokenExpiryMinutes);
        SetCookie(response, "refreshToken", refreshToken, _jwtConfig.RefreshTokenExpiryDays * 1440);
    }

    private void SetCookie(HttpResponse response, string name, string value, int expiryMinutes)
    {
        response.Cookies.Append(name, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Path = "/"
        });
    }

    public void ClearAuthCookies(HttpResponse response)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        };

        response.Cookies.Delete("accessToken", options);
        response.Cookies.Delete("refreshToken", options);
    }
}