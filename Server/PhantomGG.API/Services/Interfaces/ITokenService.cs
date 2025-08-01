using PhantomGG.API.Models;
using System.Security.Claims;

namespace PhantomGG.API.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashToBase64(string tokenHash);
    ClaimsPrincipal? ValidateToken(string token);
}