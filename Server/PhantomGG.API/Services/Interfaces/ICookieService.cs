using Microsoft.AspNetCore.Http;
using PhantomGG.API.Config;
using PhantomGG.API.DTOs.Auth.Responses;

namespace PhantomGG.API.Services.Interfaces;

public interface ICookieService
{
    void SetAuthCookies(HttpResponse response, TokenResponse tokenResponse, bool rememberMe = false);
    void ClearAuthCookies(HttpResponse response);
}