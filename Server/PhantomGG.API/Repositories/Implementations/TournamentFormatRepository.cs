using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;

namespace PhantomGG.API.Repositories.Implementations;

public class TournamentFormatRepository : ITournamentFormatRepository
{
    private readonly PhantomContext _context;

    public TournamentFormatRepository(PhantomContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TournamentFormat>> GetAllAsync()
    {
        return await _context.TournamentFormats
            .Where(tf => tf.IsActive)
            .OrderBy(tf => tf.Name)
            .ToListAsync();
    }

    public async Task<TournamentFormat?> GetByIdAsync(Guid id)
    {
        return await _context.TournamentFormats
            .FirstOrDefaultAsync(tf => tf.Id == id && tf.IsActive);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.TournamentFormats
            .AnyAsync(tf => tf.Id == id && tf.IsActive);
    }
}
