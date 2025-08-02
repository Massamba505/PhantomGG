using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;

namespace PhantomGG.API.Repositories.Implementations;

public class RefreshTokenRepository(PhantomGGContext context) : IRefreshTokenRepository
{
    private readonly PhantomGGContext _context = context;

    public async Task AddAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);
    }

    public async Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.Expires > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task RevokeAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }
}