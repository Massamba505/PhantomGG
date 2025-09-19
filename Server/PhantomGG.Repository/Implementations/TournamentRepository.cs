using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Models.Entities;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

public class TournamentRepository(PhantomContext context) : ITournamentRepository
{
    private readonly PhantomContext _context = context;

    #region Tournament Query Operations

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        return await _context.Tournaments
            .Include(t => t.Organizer)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tournament>> GetAllAsync()
    {
        return await _context.Tournaments
            .Include(t => t.Organizer)
            .Where(t => t.IsPublic)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId)
    {
        return await _context.Tournaments
            .Include(t => t.Organizer)
            .Where(t => t.OrganizerId == organizerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tournament>> GetMyTournamentsAsync(Guid userId)
    {
        var organizerTournaments = _context.Tournaments
            .Include(t => t.Organizer)
            .Where(t => t.OrganizerId == userId);

        var participantTournaments = _context.Tournaments
            .Include(t => t.Organizer)
            .Where(t => t.TournamentTeams.Any(tt => tt.Team.UserId == userId));

        var allTournaments = await organizerTournaments
            .Union(participantTournaments)
            .Distinct()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return allTournaments;
    }

    #endregion

    #region Tournament Search & Filtering

    public async Task<PaginatedResponse<Tournament>> SearchAsync(TournamentSearchDto searchDto, Guid? organizerId)
    {
        var query = _context.Tournaments
            .Include(t => t.Organizer)
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
                                    (t.Description != null && t.Description.Contains(searchDto.SearchTerm)));
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

        return new PaginatedResponse<Tournament>(
            tournaments,
            searchDto.PageNumber,
            searchDto.PageSize,
            totalRecords
        );
    }

    #endregion

    #region Tournament CRUD Operations

    public async Task<Tournament> CreateAsync(Tournament tournament)
    {
        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();
        return tournament;
    }

    public async Task<Tournament> UpdateAsync(Tournament tournament)
    {
        var existing = await _context.Tournaments.FindAsync(tournament.Id);
        if (existing == null)
            throw new ArgumentException("Tournament not found");

        tournament.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existing).CurrentValues.SetValues(tournament);
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

    #endregion

    #region Team Management Operations

    public async Task<IEnumerable<Team>> GetTournamentTeamsAsync(Guid tournamentId)
    {
        return await _context.Teams
            .Include(t => t.User)
            .Where(t => t.TournamentTeams.Any(tt => tt.TournamentId == tournamentId))
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetPendingTeamsAsync(Guid tournamentId)
    {
        return await _context.Teams
            .Include(t => t.User)
            .Where(t => t.TournamentTeams.Any(tt => tt.TournamentId == tournamentId && tt.Status == "Pending"))
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetApprovedTeamsAsync(Guid tournamentId)
    {
        return await _context.Teams
            .Include(t => t.User)
            .Where(t => t.TournamentTeams.Any(tt => tt.TournamentId == tournamentId && tt.Status == "Approved"))
            .OrderBy(t => t.Name)
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
            .FirstOrDefaultAsync(tt => tt.TournamentId == tournamentId && tt.TeamId == teamId);
    }

    public async Task UpdateTeamRegistrationStatusAsync(Guid tournamentId, Guid teamId, string status, string? reason = null)
    {
        var registration = await _context.TournamentTeams
            .FirstOrDefaultAsync(tt => tt.TournamentId == tournamentId && tt.TeamId == teamId);

        if (registration == null)
            throw new ArgumentException("Team registration not found");

        registration.Status = status;
        if (status == "Approved")
        {
            registration.AcceptedAt = DateTime.UtcNow;
        }
        // Note: TournamentTeam entity doesn't have UpdatedAt field
        // If you need to track reason, consider adding a Reason field to the entity

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Validation Operations

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Tournaments.AnyAsync(t => t.Id == id);
    }

    public async Task<bool> IsOrganizerAsync(Guid tournamentId, Guid userId)
    {
        return await _context.Tournaments
            .AnyAsync(t => t.Id == tournamentId && t.OrganizerId == userId);
    }

    public async Task<int> GetApprovedTeamCountAsync(Guid tournamentId)
    {
        return await _context.TournamentTeams
            .CountAsync(tt => tt.TournamentId == tournamentId && tt.Status == "Approved");
    }

    #endregion
}
