using Microsoft.EntityFrameworkCore;
using PhantomGG.Common.Enums;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

public class TournamentMatchRepository(PhantomContext context) : ITournamentMatchRepository
{
    private readonly PhantomContext _context = context;

    public async Task<IEnumerable<Match>> GetByTournamentAsync(Guid tournamentId)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
                .ThenInclude(t => t.Players)
            .Include(m => m.AwayTeam)
                .ThenInclude(t => t.Players)
            .Include(m => m.MatchEvents)
                .ThenInclude(me => me.Player)
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }
    public async Task<IEnumerable<Match>> GetByTournamentAndStatusAsync(Guid tournamentId, string status)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
                .ThenInclude(t => t.Players)
            .Include(m => m.AwayTeam)
                .ThenInclude(t => t.Players)
            .Include(m => m.MatchEvents)
                .ThenInclude(me => me.Player)
            .Where(m => m.TournamentId == tournamentId && m.Status == status)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchEvent>> GetEventsByTournamentAsync(Guid tournamentId)
    {
        return await _context.MatchEvents
            .Include(me => me.Player)
            .Include(me => me.Team)
                .ThenInclude(t => t.Players)
            .Include(me => me.Match)
            .Where(me => me.Match.TournamentId == tournamentId)
            .OrderBy(me => me.Match.MatchDate)
            .ThenBy(me => me.Minute)
            .ToListAsync();
    }
}
