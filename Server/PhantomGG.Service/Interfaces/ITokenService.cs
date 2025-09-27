using PhantomGG.Models.DTOs.AuthToken;
using PhantomGG.Models.Entities;

namespace PhantomGG.Service.Interfaces;

public interface ITokenService
{
    AccessTokenDto GenerateAccessToken(User user);
    RefreshTokenDto GenerateRefreshToken();
    DateTime GetAccessTokenExpiry(DateTime dateTime);
    DateTime GetRefreshTokenExpiry(DateTime dateTime);
}
