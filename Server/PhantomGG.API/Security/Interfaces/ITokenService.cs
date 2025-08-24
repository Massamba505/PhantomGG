using PhantomGG.API.DTOs.AuthToken;
using PhantomGG.API.Models;

namespace PhantomGG.API.Security.Interfaces;

public interface ITokenService
{
    AccessTokenDto GenerateAccessToken(User user);
    RefreshTokenDto GenerateRefreshToken();
    DateTime GetAccessTokenExpiry(DateTime dateTime);
    DateTime GetRefreshTokenExpiry(DateTime dateTime);
}
