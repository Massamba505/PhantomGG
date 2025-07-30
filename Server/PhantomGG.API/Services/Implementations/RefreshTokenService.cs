using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _tokenRepo;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(
        IRefreshTokenRepository tokenRepo,
        ITokenService tokenService,
        ILogger<RefreshTokenService> logger)
    {
        _tokenRepo = tokenRepo;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task CreateRefreshTokenAsync(Guid userId, string token)
    {
        var tokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = _tokenService.HashToken(token),
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _tokenRepo.CreateAsync(tokenEntity);
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var tokenHash = _tokenService.HashToken(token);
        var tokenEntity = await _tokenRepo.GetByTokenHashAsync(tokenHash);

        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.Expires < DateTime.UtcNow)
            return null;

        return tokenEntity;
    }

    public async Task RevokeRefreshTokenAsync(Guid tokenId)
    {
        var token = await _tokenRepo.GetByIdAsync(tokenId);
        if (token == null) return;

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _tokenRepo.UpdateAsync(token);
    }
}