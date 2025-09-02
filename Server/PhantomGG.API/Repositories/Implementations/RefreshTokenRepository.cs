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
            .Where(rt => rt.UserId == userId && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task CreateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RefreshToken token)
    {
        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllForUserAsync(Guid userId)
    {
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(userTokens);
        await _context.SaveChangesAsync();
    }
}
