using PhantomGG.API.Config;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class CookieService(JwtConfig jwtConfig ) : ICookieService
{
    private readonly JwtConfig _jwtConfig = jwtConfig;
    public void SetAuthCookies(HttpResponse response, TokenResponse tokenResponse, bool rememberMe = false)
    {        
        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
        };
        
        if (rememberMe)
        {
            refreshCookieOptions.Expires = tokenResponse.RefreshTokenExpires;
            refreshCookieOptions.MaxAge = TimeSpan.FromDays(_jwtConfig.RefreshTokenExpiryDays);
        }
        
        response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, refreshCookieOptions);
        
        response.Cookies.Append("tokenExpires", tokenResponse.AccessTokenExpires.ToString("o"), new CookieOptions
        {
            HttpOnly = false, 
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = rememberMe ? tokenResponse.AccessTokenExpires : null,
            Path = "/"
        });
    }

    public void ClearAuthCookies(HttpResponse response)
    {        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        response.Cookies.Delete("refreshToken", cookieOptions);
        response.Cookies.Delete("tokenExpires", cookieOptions);
    }
}