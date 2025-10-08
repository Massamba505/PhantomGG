using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

public class TournamentTeamRepository(PhantomContext context) : ITournamentTeamRepository
{
    private readonly PhantomContext _context = context;

    public async Task<IEnumerable<TournamentTeam>> GetByTournamentAsync(Guid tournamentId)
    {
        return await _context.TournamentTeams
            .Include(tt => tt.Team)
                .ThenInclude(t => t.User)
            .Include(tt => tt.Team)
                .ThenInclude(t => t.Players)
            .Where(tt => tt.TournamentId == tournamentId)
            .OrderBy(tt => tt.Team.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<TournamentTeam>> GetByTournamentAndStatusAsync(Guid tournamentId, string status)
    {
        return await _context.TournamentTeams
            .Include(tt => tt.Team)
                .ThenInclude(t => t.User)
            .Include(tt => tt.Team)
                .ThenInclude(t => t.Players)
            .Where(tt => tt.TournamentId == tournamentId && tt.Status == status)
            .OrderBy(tt => tt.RequestedAt)
            .ToListAsync();
    }

    public async Task<TournamentTeam?> GetRegistrationAsync(Guid tournamentId, Guid teamId)
    {
        return await _context.TournamentTeams
            .Include(tt => tt.Team)
            .Include(tt => tt.Tournament)
            .FirstOrDefaultAsync(tt => tt.TournamentId == tournamentId && tt.TeamId == teamId);
    }

    public async Task<bool> IsTeamRegisteredAsync(Guid tournamentId, Guid teamId)
    {
        return await _context.TournamentTeams
            .AnyAsync(tt => tt.TournamentId == tournamentId && tt.TeamId == teamId);
    }

    public async Task<TournamentTeam> CreateAsync(TournamentTeam tournamentTeam)
    {
        _context.TournamentTeams.Add(tournamentTeam);
        await _context.SaveChangesAsync();
        return tournamentTeam;
    }

    public async Task<TournamentTeam> UpdateAsync(TournamentTeam tournamentTeam)
    {
        _context.Update(tournamentTeam);
        await _context.SaveChangesAsync();
        return tournamentTeam;
    }

    public async Task DeleteAsync(TournamentTeam tournamentTeam)
    {
        _context.TournamentTeams.Remove(tournamentTeam);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCountByStatusAsync(Guid tournamentId, string status)
    {
        return await _context.TournamentTeams
            .CountAsync(tt => tt.TournamentId == tournamentId && tt.Status == status);
    }

    public async Task<IEnumerable<Tournament>> GetTournamentsByTeamAsync(Guid teamId)
    {
        return await _context.Tournaments
            .Include(t => t.TournamentTeams)
            .Where(t => t.TournamentTeams.Any(tt => tt.TeamId == teamId))
            .OrderBy(t => t.StartDate)
            .ToListAsync();
    }
}
