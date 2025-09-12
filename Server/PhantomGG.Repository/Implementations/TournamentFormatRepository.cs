using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Models.Entities;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

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
