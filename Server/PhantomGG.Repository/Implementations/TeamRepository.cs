using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Entities;
using PhantomGG.Models.DTOs;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Specifications;

namespace PhantomGG.Repository.Implementations;

public class TeamRepository(PhantomContext context) : ITeamRepository
{
    private readonly PhantomContext _context = context;

    public async Task<Team?> GetByIdAsync(Guid teamId)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == teamId);
    }

    public async Task<IEnumerable<Team>> GetByUserAsync(Guid userId)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Team> CreateAsync(Team team)
    {
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        return team;
    }

    public async Task<Team> UpdateAsync(Team team)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync();
        return team;
    }

    public async Task DeleteAsync(Guid teamId)
    {
        var team = await _context.Teams.FindAsync(teamId);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PagedResult<Team>> SearchAsync(TeamSpecification specification)
    {
        var query = _context.Teams
            .Include(t => t.Players)
            .Include(t => t.TournamentTeams)
            .Where(specification.ToExpression())
            .OrderByDescending(t => t.CreatedAt)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var teams = await query
            .Skip((specification.Page - 1) * specification.PageSize)
            .Take(specification.PageSize)
            .ToListAsync();

        return new PagedResult<Team>(teams, specification.Page, specification.PageSize, totalCount);
    }

    public async Task<bool> IsTeamNameUniqueInTournamentAsync(string teamName, Guid tournamentId, Guid? excludeTeamId = null)
    {
        var query = _context.Teams
            .Where(t => t.Name.ToLower() == teamName.ToLower() &&
                       t.TournamentTeams.Any(tt => tt.TournamentId == tournamentId));

        if (excludeTeamId.HasValue)
        {
            query = query.Where(t => t.Id != excludeTeamId.Value);
        }

        return !await query.AnyAsync();
    }
}
