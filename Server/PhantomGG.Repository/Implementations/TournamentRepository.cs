using Microsoft.EntityFrameworkCore;
using PhantomGG.Models.DTOs;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Specifications;

namespace PhantomGG.Repository.Implementations;

public class TournamentRepository(PhantomContext context) : ITournamentRepository
{
    private readonly PhantomContext _context = context;

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        return await _context.Tournaments
            .Include(t => t.Organizer)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tournament> CreateAsync(Tournament tournament)
    {
        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();
        return tournament;
    }

    public async Task<Tournament> UpdateAsync(Tournament tournament)
    {
        _context.Update(tournament);
        await _context.SaveChangesAsync();
        return tournament;
    }

    public async Task DeleteAsync(Guid id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);
        if (tournament == null) return;

        _context.Tournaments.Remove(tournament);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<Tournament>> SearchAsync(TournamentSpecification spec)
    {
        var query = _context.Tournaments
            .Include(t => t.Organizer)
            .Where(spec.ToExpression());

        var totalRecords = await query.CountAsync();

        var tournaments = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((spec.Page - 1) * spec.PageSize)
            .Take(spec.PageSize)
            .ToListAsync();

        return new PagedResult<Tournament>(tournaments, spec.Page, spec.PageSize, totalRecords);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Tournaments.AnyAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId)
    {
        return await _context.Tournaments
            .Where(t => t.OrganizerId == organizerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
