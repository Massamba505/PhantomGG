using Microsoft.IdentityModel.Tokens;
using PhantomGG.API.Config;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhantomGG.API.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly JwtUtils _jwtUtils;
    private readonly JwtConfig _jwtConfig;
    private readonly IRefreshTokenRepository _tokenRepository;

    public TokenService(JwtConfig jwtConfig, 
           JwtUtils jwtUtils,
           IRefreshTokenRepository tokenRepository)
    {
        _jwtConfig = jwtConfig;
        _jwtUtils = jwtUtils;
        _tokenRepository = tokenRepository;
    }

    public async Task<TokenPair> GenerateAuthResponseAsync(User user)
    {
        var accessToken = _jwtUtils.GenerateAccessToken(user);
        var refreshTokenRaw = _jwtUtils.GenerateRefreshToken();
        var refreshTokenHash = _jwtUtils.HashRefreshToken(refreshTokenRaw);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            CreatedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryDays)
        };

        await _tokenRepository.AddAsync(refreshToken);

        return new TokenPair
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenRaw,
        };
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token)) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}