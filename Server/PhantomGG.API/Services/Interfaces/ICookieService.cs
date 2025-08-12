using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

public interface ICookieService
{
    void SetAuthCookies(HttpResponse response, TokenResponse tokenResponse, bool rememberMe = false);
    void ClearAuthCookies(HttpResponse response);
}