using PhantomGG.Common.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PhantomGG.Service.Infrastructure.Security.Interfaces;
using PhantomGG.Service.Auth.Interfaces;

namespace PhantomGG.Service.Infrastructure.Security.Implementations;

public class CookieService(
    IOptions<CookieSettings> cookieSettings,
    ITokenService tokenService) : ICookieService
{
    private readonly CookieSettings _cookieSettings = cookieSettings.Value;
    private readonly ITokenService _tokenService = tokenService;

    public void SetRefreshToken(HttpResponse response, string token, bool rememberMe = true)
    {
        var now = DateTime.UtcNow;
        var dateTime = _tokenService.GetRefreshTokenExpiry(now);

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = _cookieSettings.HttpOnly,
            Secure = _cookieSettings.Secure,
            Path = _cookieSettings.Path,
            Domain = _cookieSettings.Domain,
            SameSite = SameSiteMode.Lax
        };

        if (rememberMe)
        {
            refreshCookieOptions.Expires = dateTime;
            refreshCookieOptions.MaxAge = dateTime - now;
        }

        response.Cookies.Append(_cookieSettings.RefreshTokenCookieName, token, refreshCookieOptions);
    }


    public void ClearRefreshToken(HttpResponse response)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = _cookieSettings.HttpOnly,
            Secure = _cookieSettings.Secure,
            Path = _cookieSettings.Path,
            Expires = DateTime.UtcNow.AddDays(-1),
            Domain = _cookieSettings.Domain,
            SameSite = SameSiteMode.Lax
        };

        response.Cookies.Append(_cookieSettings.RefreshTokenCookieName, "", cookieOptions);
    }
}
