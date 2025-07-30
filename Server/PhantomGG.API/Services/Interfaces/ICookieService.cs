using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

public interface ICookieService
{
    void SetAuthCookies(HttpResponse response, TokenPair tokens);
    void ClearAuthCookies(HttpResponse response);
}