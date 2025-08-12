using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;

namespace PhantomGG.API.Repositories.Implementations;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task AddAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }
}