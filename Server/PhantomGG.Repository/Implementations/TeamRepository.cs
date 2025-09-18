using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Models.Entities;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

public class TeamRepository(PhantomContext context) : ITeamRepository
{
    private readonly PhantomContext _context = context;

    #region Team Query Operations

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _context.Teams
            .Include(t => t.Players)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .Include(t => t.TournamentTeams)
            .ThenInclude(tt => tt.Tournament)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Team>> GetByUserAsync(Guid userId)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .Include(t => t.TournamentTeams)
            .ThenInclude(tt => tt.Tournament)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetByTournamentAsync(Guid tournamentId)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .Include(t => t.TournamentTeams)
            .Where(t => t.TournamentTeams.Any(tt => tt.TournamentId == tournamentId))
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetByTournamentAndStatusAsync(Guid tournamentId, string status)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .Include(t => t.TournamentTeams)
            .Where(t => t.TournamentTeams.Any(tt => tt.TournamentId == tournamentId && tt.Status == status))
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tournament>> GetTournamentsByTeamAsync(Guid teamId)
    {
        return await _context.Tournaments
            .Include(t => t.TournamentTeams)
            .Where(t => t.TournamentTeams.Any(tt => tt.TeamId == teamId))
            .OrderBy(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<int> GetTournamentTeamCountAsync(Guid tournamentId)
    {
        return await _context.TournamentTeams
            .CountAsync(tt => tt.TournamentId == tournamentId);
    }

    #endregion

    #region Team CRUD Operations

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

    public async Task DeleteAsync(Guid id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Team Tournament Management

    public async Task RegisterForTournamentAsync(Guid teamId, Guid tournamentId)
    {
        var tournamentTeam = new TournamentTeam
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            TournamentId = tournamentId,
            Status = "Pending",
            RequestedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.TournamentTeams.Add(tournamentTeam);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTeamTournamentStatusAsync(Guid tournamentId, Guid teamId, string status)
    {
        var tournamentTeam = await _context.TournamentTeams
            .FirstOrDefaultAsync(tt => tt.TournamentId == tournamentId && tt.TeamId == teamId);

        if (tournamentTeam != null)
        {
            tournamentTeam.Status = status;
            if (status == "Approved")
                tournamentTeam.AcceptedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromTournamentAsync(Guid tournamentId, Guid teamId)
    {
        var tournamentTeam = await _context.TournamentTeams
            .FirstOrDefaultAsync(tt => tt.TournamentId == tournamentId && tt.TeamId == teamId);

        if (tournamentTeam != null)
        {
            _context.TournamentTeams.Remove(tournamentTeam);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Team Search Operations

    public async Task<PaginatedResponse<Team>> SearchAsync(TeamSearchDto searchDto)
    {
        var query = _context.Teams
            .Include(t => t.User)
            .Include(t => t.Players)
            .AsQueryable();

        // Apply search term filter
        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchDto.SearchTerm) ||
                                    (t.ShortName != null && t.ShortName.Contains(searchDto.SearchTerm)));
        }

        // Apply tournament filter
        if (searchDto.TournamentId.HasValue)
        {
            query = query.Where(t => t.TournamentTeams.Any(tt => tt.TournamentId == searchDto.TournamentId.Value));
        }

        // Get total count
        var totalRecords = await query.CountAsync();

        // Apply sorting and pagination
        var teams = await query
            .OrderBy(t => t.Name)
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return new PaginatedResponse<Team>(
            teams,
            searchDto.Page,
            searchDto.PageSize,
            totalRecords
        );
    }

    #endregion

    #region Team Player Management

    public async Task<IEnumerable<Player>> GetTeamPlayersAsync(Guid teamId)
    {
        return await _context.Players
            .Where(p => p.TeamId == teamId)
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .ToListAsync();
    }

    public async Task<int> GetTeamPlayerCountAsync(Guid teamId)
    {
        return await _context.Players
            .CountAsync(p => p.TeamId == teamId);
    }

    #endregion

    #region Validation Operations

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Teams.AnyAsync(t => t.Id == id);
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

    public async Task<bool> IsUserTeamOwnerAsync(Guid teamId, Guid userId)
    {
        return await _context.Teams
            .AnyAsync(t => t.Id == teamId && t.UserId == userId);
    }

    #endregion
}
