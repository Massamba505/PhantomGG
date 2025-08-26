using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.DTOs.Team;

namespace PhantomGG.API.Repositories.Implementations;

public class TeamRepository : ITeamRepository
{
    private readonly PhantomContext _context;

    public TeamRepository(PhantomContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _context.Teams
            .Include(t => t.Tournament)
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .Include(t => t.Tournament)
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
    }

    public async Task<IEnumerable<Team>> GetByManagerAsync(string manager)
    {
        return await _context.Teams
            .Include(t => t.Tournament)
            .Where(t => t.Manager == manager && t.IsActive)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetByTournamentAsync(Guid tournamentId)
    {
        return await _context.Teams
            .Where(t => t.TournamentId == tournamentId && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> SearchAsync(TeamSearchDto searchDto)
    {
        var query = _context.Teams
            .Include(t => t.Tournament)
            .Where(t => t.IsActive);

        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchDto.SearchTerm) ||
                                   t.Manager.Contains(searchDto.SearchTerm));
        }

        if (searchDto.TournamentId.HasValue)
        {
            query = query.Where(t => t.TournamentId == searchDto.TournamentId.Value);
        }

        query = query.OrderByDescending(t => t.CreatedAt);

        if (searchDto.PageSize > 0)
        {
            query = query.Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                         .Take(searchDto.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<Team> CreateAsync(Team team)
    {
        team.CreatedAt = DateTime.UtcNow;
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        return team;
    }

    public async Task<Team> UpdateAsync(Team team)
    {
        team.UpdatedAt = DateTime.UtcNow;
        _context.Teams.Update(team);
        await _context.SaveChangesAsync();
        return team;
    }

    public async Task DeleteAsync(Guid id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Teams
            .AnyAsync(t => t.Id == id && t.IsActive);
    }

    public async Task<bool> TeamNameExistsInTournamentAsync(string name, Guid tournamentId, Guid? excludeId = null)
    {
        var query = _context.Teams
            .Where(t => t.Name == name && t.TournamentId == tournamentId && t.IsActive);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
