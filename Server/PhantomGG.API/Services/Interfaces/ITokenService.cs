namespace PhantomGG.API.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string roleName);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
    string? GetEmailFromToken(string token);
    DateTime GetTokenExpiration(string token);
}
