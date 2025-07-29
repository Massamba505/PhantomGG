namespace PhantomGG.API.Services.Interfaces;

public interface ICookieService
{
    void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken);
    void ClearAuthCookies(HttpResponse response);
}
