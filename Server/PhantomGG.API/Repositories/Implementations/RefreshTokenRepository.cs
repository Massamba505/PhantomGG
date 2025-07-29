using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Repositories.Implementations;

public class RefreshTokenRepository(PhantomGGContext context, ITokenService tokenService) : IRefreshTokenRepository
{
    private readonly PhantomGGContext _context = context;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<IEnumerable<RefreshToken>> GetAllAsync()
    {
        return await _context.RefreshTokens.ToListAsync();
    }

    public async Task<RefreshToken?> GetByIdAsync(Guid tokenId)
    {
        return await _context.RefreshTokens.FindAsync(tokenId);
    }

    public async Task<IEnumerable<RefreshToken>> GetTokensByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string token)
    {
        var hashedToken = _tokenService.HashToken(token);
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == hashedToken &&
                !rt.IsRevoked &&
                rt.Expires > DateTime.UtcNow);
    }

    public async Task CreateAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid tokenId)
    {
        var token = await _context.RefreshTokens.FindAsync(tokenId);
        if (token != null)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RevokeAsync(Guid tokenId)
    {
        var token = await _context.RefreshTokens.FindAsync(tokenId);
        if (token != null)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.Expires < DateTime.UtcNow)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}