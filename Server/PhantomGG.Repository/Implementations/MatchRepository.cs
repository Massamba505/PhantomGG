using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Specifications;

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
            .Include(m => m.Tournament)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetByTournamentAndStatusAsync(Guid tournamentId, string status)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.TournamentId == tournamentId &&
                       m.MatchDate > DateTime.UtcNow &&
                       m.Status == status)
            .Include(m => m.Tournament)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<PagedResult<Match>> SearchAsync(MatchSpecification specification)
    {
        var query = _context.Matches
            .Include(m => m.Tournament)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(specification.ToExpression());

        var totalCount = await query.CountAsync();

        var matches = await query
            .OrderBy(m => m.MatchDate)
            .Skip((specification.Page - 1) * specification.PageSize)
            .Take(specification.PageSize)
            .ToListAsync();

        return new PagedResult<Match>(matches, specification.Page, specification.PageSize, totalCount);
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
}
