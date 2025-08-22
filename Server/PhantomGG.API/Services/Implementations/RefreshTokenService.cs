using PhantomGG.API.DTOs.AuthToken;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService) : IRefreshTokeService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly ITokenService _tokenService = tokenService;


    public async Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _refreshTokenRepository.GetActiveByUserIdAsync(userId);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _refreshTokenRepository.GetByTokenAsync(token);
    }

    public async Task<RefreshTokenDto> AddRefreshToken(User user)
    {
        var refreshToken = _tokenService.GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(newRefreshTokenEntity);

        return new RefreshTokenDto
        {
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt
        };
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        await _refreshTokenRepository.RevokeAllForUserAsync(userId);
    }

    public async Task<RefreshToken> RevokeAsync(string refreshTokenFromCookie)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenFromCookie))
        {
            throw new UnauthorizedException("Refresh token is required");
        }

        var refreshTokenEntity = await GetByTokenAsync(refreshTokenFromCookie);
        if (refreshTokenEntity == null)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        if (refreshTokenEntity.RevokedAt.HasValue)
        {
            await RevokeAllForUserAsync(refreshTokenEntity.UserId);
            throw new UnauthorizedException("Token reuse detected. All tokens revoked.");
        }

        if (refreshTokenEntity.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token has expired");
        }

        await _refreshTokenRepository.RevokeAsync(refreshTokenEntity);

        return refreshTokenEntity;
    }
}
