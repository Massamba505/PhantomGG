using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.DTOs.Tournament;

namespace PhantomGG.API.Repositories.Implementations;

public class TournamentRepository : ITournamentRepository
{
    private readonly PhantomContext _context;

    public TournamentRepository(PhantomContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tournament>> GetAllAsync()
    {
        return await _context.Tournaments
            .Include(t => t.OrganizerNavigation)
            .Include(t => t.Teams)
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        return await _context.Tournaments
            .Include(t => t.OrganizerNavigation)
            .Include(t => t.Teams)
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
    }

    public async Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId)
    {
        return await _context.Tournaments
            .Include(t => t.OrganizerNavigation)
            .Include(t => t.Teams)
            .Where(t => t.Organizer == organizerId && t.IsActive)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tournament>> SearchAsync(TournamentSearchDto searchDto)
    {
        var query = _context.Tournaments
            .Include(t => t.OrganizerNavigation)
            .Include(t => t.Teams)
            .Where(t => t.IsActive);

        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchDto.SearchTerm) ||
                                   t.Description.Contains(searchDto.SearchTerm));
        }

        if (!string.IsNullOrEmpty(searchDto.Status))
        {
            query = query.Where(t => t.Status == searchDto.Status);
        }

        query = query.OrderByDescending(t => t.CreatedAt);

        if (searchDto.PageSize > 0)
        {
            query = query.Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                         .Take(searchDto.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<Tournament> CreateAsync(Tournament tournament)
    {
        tournament.CreatedAt = DateTime.UtcNow;
        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();
        return tournament;
    }

    public async Task<Tournament> UpdateAsync(Tournament tournament)
    {
        _context.Tournaments.Update(tournament);
        await _context.SaveChangesAsync();
        return tournament;
    }

    public async Task DeleteAsync(Guid id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);
        if (tournament != null)
        {
            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Tournaments
            .AnyAsync(t => t.Id == id && t.IsActive);
    }

    public async Task<bool> IsOrganizerAsync(Guid tournamentId, Guid userId)
    {
        return await _context.Tournaments
            .AnyAsync(t => t.Id == tournamentId && t.Organizer == userId && t.IsActive);
    }

    public async Task<int> GetTeamCountAsync(Guid tournamentId)
    {
        return await _context.Teams
            .CountAsync(t => t.TournamentId == tournamentId && t.IsActive);
    }
}
