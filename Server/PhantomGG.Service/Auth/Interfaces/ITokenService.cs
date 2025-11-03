using PhantomGG.Models.DTOs.AuthToken;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Auth.Interfaces;

public interface ITokenService
{
    AccessTokenDto GenerateAccessToken(User user);
    RefreshTokenDto GenerateRefreshToken();
    EmailVerificationTokenDto GenerateEmailVerificationToken();
    PasswordResetTokenDto GeneratePasswordResetToken();
    DateTime GetAccessTokenExpiry(DateTime dateTime);
    DateTime GetRefreshTokenExpiry(DateTime dateTime);
    DateTime GetEmailVerificationTokenExpiry(DateTime dateTime);
    DateTime GetPasswordResetTokenExpiry(DateTime dateTime);
}
