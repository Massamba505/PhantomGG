using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Entities;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

public class TeamRepository(PhantomContext context) : ITeamRepository
{
    private readonly PhantomContext _context = context;

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _context.Teams
            .Include(t => t.Players)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

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

    public async Task<PaginatedResult<Team>> SearchAsync(TeamSearchDto searchDto, Guid? userId)
    {
        var query = _context.Teams
            .Include(t => t.Players)
            .OrderByDescending(t => t.CreatedAt)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(t => t.UserId == userId.Value);
        }

        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchDto.SearchTerm) ||
                                    t.ShortName.Contains(searchDto.SearchTerm));
        }

        if (searchDto.TournamentId.HasValue)
        {
            query = query.Where(t => t.TournamentTeams.Any(tt => tt.TournamentId == searchDto.TournamentId.Value));
        }

        var totalRecords = await query.CountAsync();

        var teams = await query
            .OrderBy(t => t.Name)
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return new PaginatedResult<Team>(teams, totalRecords); ;
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
