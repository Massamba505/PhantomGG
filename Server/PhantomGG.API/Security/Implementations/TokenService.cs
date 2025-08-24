using PhantomGG.API.Config;
using PhantomGG.API.DTOs.AuthToken;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Security.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhantomGG.API.Security.Implementations;

public class TokenService(
    IOptions<JwtSettings> jwtSettings,
    IRefreshTokenRepository refreshTokenRepository) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    private string GenerateAccessTokenString(User user, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public AccessTokenDto GenerateAccessToken(User user)
    {
        var expiresAt = GetAccessTokenExpiry(DateTime.UtcNow);

        return new AccessTokenDto
        {
            Token = GenerateAccessTokenString(user, expiresAt),
            ExpiresAt = expiresAt,
        };
    }

    public DateTime GetAccessTokenExpiry(DateTime dateTime)
    {
        return dateTime.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes);
    }

    private string GenerateRefreshTokenString()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    public RefreshTokenDto GenerateRefreshToken()
    {
        var currentdate = DateTime.UtcNow;

        return new RefreshTokenDto
        {
            Token = GenerateRefreshTokenString(),
            ExpiresAt = GetRefreshTokenExpiry(currentdate)
        };
    }

    public DateTime GetRefreshTokenExpiry(DateTime dateTime)
    {
        return dateTime.AddDays(_jwtSettings.RefreshTokenLifetimeDays);
    }
}
