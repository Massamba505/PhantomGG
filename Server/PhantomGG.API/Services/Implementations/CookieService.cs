using PhantomGG.API.Config;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

/// <summary>
/// Implementation of the cookie service
/// </summary>
public class CookieService : ICookieService
{
    private readonly JwtConfig _config;
    
    /// <summary>
    /// Initializes a new instance of the CookieService
    /// </summary>
    /// <param name="config">JWT configuration</param>
    public CookieService(JwtConfig config)
    {
        _config = config;
    }

    /// <inheritdoc />
    public void SetAuthCookies(HttpResponse response, TokenResponse tokenResponse)
    {
        // Set access token cookie
        response.Cookies.Append("accessToken", tokenResponse.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to true in production with HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = tokenResponse.AccessTokenExpires,
            Path = "/"
        });

        // Set refresh token cookie
        response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to true in production with HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = tokenResponse.RefreshTokenExpires,
            Path = "/"
        });
    }

    /// <inheritdoc />
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