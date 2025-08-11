using PhantomGG.API.Config;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace PhantomGG.API.Services.Implementations;

/// <summary>
/// Implementation of the cookie service
/// </summary>
public class CookieService : ICookieService
{
    private readonly JwtConfig _config;
    private readonly IHostEnvironment _environment;
    
    /// <summary>
    /// Initializes a new instance of the CookieService
    /// </summary>
    /// <param name="config">JWT configuration</param>
    /// <param name="environment">Host environment</param>
    public CookieService(JwtConfig config, IHostEnvironment environment)
    {
        _config = config;
        _environment = environment;
    }

    /// <inheritdoc />
    public void SetAuthCookies(HttpResponse response, TokenResponse tokenResponse, bool rememberMe = false)
    {
        // Determine if we're in production to set Secure flag
        bool isProduction = !_environment.IsDevelopment();
        
        // Set refresh token cookie (HTTP-only for security)
        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction, // True in production with HTTPS
            SameSite = SameSiteMode.Lax, // Lax allows the cookie to be sent with top-level navigations
            Path = "/",
        };
        
        if (rememberMe)
        {
            // For "remember me", set a persistent cookie with expiration
            refreshCookieOptions.Expires = tokenResponse.RefreshTokenExpires;
            refreshCookieOptions.MaxAge = TimeSpan.FromDays(_config.RefreshTokenExpiryDays);
        }
        // else: No Expires set = session cookie that expires when browser closes
        
        response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, refreshCookieOptions);
        
        // Don't set access token in cookie - it will be handled by the client for API calls
        // But do expose token expiration for client-side handling
        response.Cookies.Append("tokenExpires", tokenResponse.AccessTokenExpires.ToString("o"), new CookieOptions
        {
            HttpOnly = false, // This needs to be accessible from JavaScript
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,
            Expires = rememberMe ? tokenResponse.AccessTokenExpires : null,
            Path = "/"
        });
    }

    /// <inheritdoc />
    public void ClearAuthCookies(HttpResponse response)
    {
        // Determine if we're in production to set Secure flag
        bool isProduction = !_environment.IsDevelopment();
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(-1) // Expire immediately
        };

        response.Cookies.Delete("refreshToken", cookieOptions);
        response.Cookies.Delete("tokenExpires", cookieOptions);
    }
}