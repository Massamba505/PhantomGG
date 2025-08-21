using PhantomGG.API.Data;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PhantomGG.API.Repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PhantomContext _context;

    public RefreshTokenRepository(PhantomContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId &&
                        rt.RevokedAt == null &&
                        rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task CreateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAsync(RefreshToken token)
    {
        token.RevokedAt = DateTime.UtcNow;
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}
