using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<string> CreateRefreshTokenAsync(Guid userId)
    {
        var tokenValue = _tokenService.GenerateRefreshToken();
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = _tokenService.HashToken(tokenValue),
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(token);
        return tokenValue;
    }

    public async Task RevokeTokenAsync(Guid tokenId)
    {
        await _refreshTokenRepository.RevokeAsync(tokenId);
    }

    public async Task RevokeAllTokensForUserAsync(Guid userId)
    {
        var tokens = await _refreshTokenRepository.GetTokensByUserIdAsync(userId);
        foreach (var token in tokens.Where(t => !t.IsRevoked))
        {
            await _refreshTokenRepository.RevokeAsync(token.Id);
        }
    }

    public async Task<RefreshToken> ValidateTokenAsync(string token)
    {
        var validToken = await _refreshTokenRepository.GetValidTokenAsync(token);
        if (validToken == null) return null;

        // Include user information
        validToken.User = await _userRepository.GetByIdAsync(validToken.UserId);
        return validToken;
    }

    public async Task<IEnumerable<RefreshToken>> GetTokensForUserAsync(Guid userId)
    {
        return await _refreshTokenRepository.GetTokensByUserIdAsync(userId);
    }

    public async Task DeleteExpiredTokensAsync()
    {
        await _refreshTokenRepository.DeleteExpiredTokensAsync();
    }

    public Task<string> RotateTokenAsync(string oldToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenExpiringSoonAsync(Guid tokenId)
    {
        throw new NotImplementedException();
    }
}