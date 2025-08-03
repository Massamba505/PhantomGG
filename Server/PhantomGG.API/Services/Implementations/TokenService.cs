using PhantomGG.API.Config;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Utils;

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
    
    public async Task<TokenPair> RefreshTokenAsync(string refreshToken)
    {
        var tokenHash = _jwtUtils.HashRefreshToken(refreshToken);
        var token = await _tokenRepository.GetByTokenHashAsync(tokenHash);

        if (token == null || token.Expires < DateTime.UtcNow || token.IsRevoked) { 
            throw new Exception("Invalid refresh token");
        }

        return await GenerateAuthResponseAsync(token.User);
    }

    public async Task RevokeRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var tokenHash = _jwtUtils.HashRefreshToken(refreshToken);
        var token = await _tokenRepository.GetByTokenHashAsync(tokenHash);

        if (token == null)
        {
            throw new Exception("Refresh token not found");
        }

        if (token.UserId != userId)
        {
            throw new UnauthorizedAccessException("Not authorized to revoke this token");
        }

        await _tokenRepository.RevokeAsync(token);
    }
}