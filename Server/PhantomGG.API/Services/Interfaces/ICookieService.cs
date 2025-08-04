using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

public interface ICookieService
{
    void SetAuthCookies(HttpResponse response, AuthResult authResult);
    void ClearAuthCookies(HttpResponse response);
}