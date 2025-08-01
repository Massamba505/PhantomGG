using PhantomGG.API.Config;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly JwtConfig _config;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        JwtConfig config)
    {
        _config = config;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task CreateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var tokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = _tokenService.HashToBase64(refreshToken),
            Expires = DateTime.UtcNow.AddDays(_config.RefreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(tokenEntity);
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var tokenHash = _tokenService.HashToBase64(token);
        var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.Expires < DateTime.UtcNow)
            return null;

        return refreshToken;
    }

    public async Task RevokeRefreshTokenAsync(Guid tokenId)
    {
        await _refreshTokenRepository.RevokeAsync(tokenId);
    }

    public async Task<RefreshToken?> GetByUserIdAsync(Guid userId)
    {
        return await _refreshTokenRepository.GetByUserIdAsync(userId);
    }
}