using Microsoft.EntityFrameworkCore;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.Entities;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

public class TournamentRepository(PhantomContext context) : ITournamentRepository
{
    private readonly PhantomContext _context = context;

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        return await _context.Tournaments
            .Include(t => t.Organizer)
            .Include(t => t.TournamentTeams)
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

    public async Task<PaginatedResult<Tournament>> SearchAsync(TournamentSearchDto searchDto, Guid? organizerId = null)
    {
        var query = _context.Tournaments
            .Include(t => t.Organizer)
            .Include(t => t.TournamentTeams)
            .AsQueryable();

        if (organizerId.HasValue)
        {
            query = query.Where(t => t.OrganizerId == organizerId.Value);
        }

        if (searchDto.IsPublic.HasValue)
        {
            query = query.Where(t => t.IsPublic == searchDto.IsPublic);
        }

        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchDto.SearchTerm) ||
                                    t.Description.Contains(searchDto.SearchTerm));
        }

        if (!string.IsNullOrEmpty(searchDto.Status))
        {
            query = query.Where(t => t.Status == searchDto.Status);
        }

        if (!string.IsNullOrEmpty(searchDto.Location))
        {
            query = query.Where(t => t.Location != null && t.Location.Contains(searchDto.Location));
        }

        if (searchDto.StartDateFrom.HasValue)
        {
            query = query.Where(t => t.StartDate >= searchDto.StartDateFrom.Value);
        }

        if (searchDto.StartDateTo.HasValue)
        {
            query = query.Where(t => t.StartDate <= searchDto.StartDateTo.Value);
        }

        var totalRecords = await query.CountAsync();

        var tournaments = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return new PaginatedResult<Tournament>(tournaments, totalRecords);
    }

    public async Task<IEnumerable<Tournament>> GetTournamentsByTeamAsync(Guid teamId)
    {
        return await _context.Tournaments
            .Include(t => t.TournamentTeams)
            .Where(t => t.TournamentTeams.Any(tt => tt.TeamId == teamId))
            .OrderBy(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TournamentTeam>> GetTournamentTeamsAsync(Guid tournamentId)
    {
        return await _context.TournamentTeams
                              .Include(tt => tt.Team)
                              .ThenInclude(t => t.User)
                              .Where(tt => tt.TournamentId == tournamentId)
                              .OrderBy(tt => tt.Team.Name)
                              .ToListAsync();
    }

    public async Task<IEnumerable<TournamentTeam>> GetTournamentTeamByStatus(Guid tournamentId, TeamRegistrationStatus status)
    {
        return await _context.TournamentTeams
            .Include(tt => tt.Team)
            .ThenInclude(t => t.User)
            .Where(tt => tt.TournamentId == tournamentId &&
                        tt.Status == status.ToString())
            .OrderBy(tt => tt.RequestedAt)
            .ToListAsync();
    }

    public async Task<bool> IsTeamRegisteredAsync(Guid tournamentId, Guid teamId)
    {
        return await _context.TournamentTeams
            .AnyAsync(tt => tt.TournamentId == tournamentId && tt.TeamId == teamId);
    }

    public async Task<TournamentTeam?> GetTeamRegistrationAsync(Guid tournamentId, Guid teamId)
    {
        return await _context.TournamentTeams
            .Include(tt => tt.Team)
            .Include(tt => tt.Tournament)
            .FirstOrDefaultAsync(tt => tt.TournamentId == tournamentId &&
                                       tt.TeamId == teamId);
    }

    public async Task RegisterTeamForTournamentAsync(TournamentTeam tournamentTeam)
    {
        _context.TournamentTeams.Add(tournamentTeam);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeTeamRegistrationStatusAsync(TournamentTeam registration, TeamRegistrationStatus status)
    {
        registration.Status = status.ToString();

        if (status == TeamRegistrationStatus.Approved)
        {
            registration.AcceptedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveTeamFromTournamentAsync(TournamentTeam registration)
    {
        _context.TournamentTeams.Remove(registration);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTeamCountAsync(Guid tournamentId, TeamRegistrationStatus status)
    {
        return await _context.TournamentTeams
            .CountAsync(tt => tt.TournamentId == tournamentId && tt.Status == status.ToString());
    }

    public async Task<IEnumerable<Match>> GetTournamentMatchesAsync(Guid tournamentId)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.MatchEvents)
                .ThenInclude(me => me.Player)
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchEvent>> GetTournamentMatchEventsAsync(Guid tournamentId)
    {
        return await _context.MatchEvents
            .Include(me => me.Player)
            .Include(me => me.Team)
            .Include(me => me.Match)
            .Where(me => me.Match.TournamentId == tournamentId)
            .OrderBy(me => me.Match.MatchDate)
            .ThenBy(me => me.Minute)
            .ToListAsync();
    }
}
