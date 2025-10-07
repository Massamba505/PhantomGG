using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Data;

namespace PhantomGG.Repository.Implementations;

public class MatchRepository(PhantomContext context) : IMatchRepository
{
    private readonly PhantomContext _context = context;

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        return await _context.Matches
            .Include(m => m.Tournament)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Match>> GetByTournamentAsync(Guid tournamentId)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetByTeamAsync(Guid teamId)
    {
        return await _context.Matches
            .Include(m => m.Tournament)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetUpcomingMatchesAsync(Guid tournamentId)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.TournamentId == tournamentId &&
                       m.MatchDate > DateTime.UtcNow &&
                       m.Status == MatchStatus.Scheduled.ToString())
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetCompletedMatchesAsync(Guid tournamentId)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.TournamentId == tournamentId && m.Status == MatchStatus.Completed.ToString())
            .OrderByDescending(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> SearchAsync(MatchSearchDto searchDto)
    {
        var query = _context.Matches
            .Include(m => m.Tournament)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(m =>
                m.HomeTeam.Name.Contains(searchDto.SearchTerm) ||
                m.AwayTeam.Name.Contains(searchDto.SearchTerm) ||
                m.Tournament.Name.Contains(searchDto.SearchTerm));
        }

        if (searchDto.TournamentId.HasValue)
        {
            query = query.Where(m => m.TournamentId == searchDto.TournamentId);
        }

        if (!string.IsNullOrEmpty(searchDto.Status))
        {
            query = query.Where(m => m.Status == searchDto.Status);
        }

        if (searchDto.DateFrom.HasValue)
        {
            query = query.Where(m => m.MatchDate >= searchDto.DateFrom);
        }

        if (searchDto.DateTo.HasValue)
        {
            query = query.Where(m => m.MatchDate <= searchDto.DateTo);
        }

        if (searchDto.Cursor.HasValue)
        {
            query = query.Where(m => m.Id.CompareTo(searchDto.Cursor.Value) > 0);
        }

        return await query
            .OrderBy(m => m.Id)
            .Take(searchDto.Limit)
            .ToListAsync();
    }

    public async Task<Match> CreateAsync(Match match)
    {
        _context.Matches.Add(match);
        await _context.SaveChangesAsync();
        return match;
    }

    public async Task<Match> UpdateAsync(Match match)
    {
        _context.Matches.Update(match);
        await _context.SaveChangesAsync();
        return match;
    }

    public async Task DeleteAsync(Guid id)
    {
        var match = await _context.Matches.FindAsync(id);
        if (match != null)
        {
            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Matches.AnyAsync(m => m.Id == id);
    }

    public async Task<bool> TeamsHaveMatchOnDateAsync(Guid homeTeamId, Guid awayTeamId, DateTime matchDate, Guid? excludeMatchId = null)
    {
        var query = _context.Matches
            .Where(m => (m.HomeTeamId == homeTeamId && m.AwayTeamId == awayTeamId) ||
                       (m.HomeTeamId == awayTeamId && m.AwayTeamId == homeTeamId))
            .Where(m => m.MatchDate.Date == matchDate.Date);

        if (excludeMatchId.HasValue)
        {
            query = query.Where(m => m.Id != excludeMatchId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<int> GetCompletedMatchCountAsync(Guid tournamentId)
    {
        return await _context.Matches
            .CountAsync(m => m.TournamentId == tournamentId &&
                m.Status == MatchStatus.Completed.ToString());
    }

    public async Task<int> GetTotalMatchCountAsync(Guid tournamentId)
    {
        return await _context.Matches
            .CountAsync(m => m.TournamentId == tournamentId);
    }
}
