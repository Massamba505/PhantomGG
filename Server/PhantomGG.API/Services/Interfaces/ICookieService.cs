using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

/// <summary>
/// Service for managing authentication cookies
/// </summary>
public interface ICookieService
{
    /// <summary>
    /// Sets authentication cookies in the HTTP response
    /// </summary>
    /// <param name="response">HTTP response</param>
    /// <param name="tokenResponse">Token response</param>
    void SetAuthCookies(HttpResponse response, TokenResponse tokenResponse);
    
    /// <summary>
    /// Clears authentication cookies from the HTTP response
    /// </summary>
    /// <param name="response">HTTP response</param>
    void ClearAuthCookies(HttpResponse response);
}