namespace PhantomGG.API.Security.Interfaces;

public interface ICookieService
{
    void SetRefreshToken(HttpResponse response, string token, bool rememberMe = true);
    void ClearRefreshToken(HttpResponse response);
}
