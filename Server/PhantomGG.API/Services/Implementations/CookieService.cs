using PhantomGG.API.Config;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class CookieService : ICookieService
{
    private readonly JwtConfig _config;
    
    public CookieService(JwtConfig config)
    {
        _config = config;
    }

    public void SetAuthCookies(HttpResponse response, AuthResult authResult)
    {
        // Set access token cookie
        response.Cookies.Append("accessToken", authResult.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to true in production with HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = authResult.AccessTokenExpires,
            Path = "/"
        });

        // Set refresh token cookie
        response.Cookies.Append("refreshToken", authResult.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to true in production with HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = authResult.RefreshTokenExpires,
            Path = "/"
        });
    }

    public void ClearAuthCookies(HttpResponse response)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to true in production with HTTPS
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(-1) // Expire immediately
        };

        response.Cookies.Delete("accessToken", cookieOptions);
        response.Cookies.Delete("refreshToken", cookieOptions);
    }
}