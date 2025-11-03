using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

public class MatchEventRepository(PhantomContext context) : IMatchEventRepository
{
    private readonly PhantomContext _context = context;

    public async Task<IEnumerable<MatchEvent>> GetByMatchIdAsync(Guid matchId)
    {
        return await _context.MatchEvents
            .Include(me => me.Team)
            .Include(me => me.Player)
            .Where(me => me.MatchId == matchId)
            .OrderBy(me => me.Minute)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchEvent>> GetByPlayerIdAsync(Guid playerId)
    {
        return await _context.MatchEvents
            .Include(me => me.Match)
            .Include(me => me.Team)
            .Where(me => me.PlayerId == playerId)
            .OrderBy(me => me.Minute)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchEvent>> GetByTeamIdAsync(Guid teamId)
    {
        return await _context.MatchEvents
            .Include(me => me.Match)
            .Include(me => me.Player)
            .Where(me => me.TeamId == teamId)
            .OrderBy(me => me.Minute)
            .ToListAsync();
    }

    public async Task<MatchEvent?> GetByIdAsync(Guid id)
    {
        return await _context.MatchEvents
            .Include(me => me.Match)
            .Include(me => me.Team)
            .Include(me => me.Player)
            .FirstOrDefaultAsync(me => me.Id == id);
    }

    public async Task<MatchEvent> CreateAsync(MatchEvent matchEvent)
    {
        matchEvent.Id = Guid.NewGuid();

        _context.MatchEvents.Add(matchEvent);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(matchEvent.Id) ?? matchEvent;
    }

    public async Task<MatchEvent> UpdateAsync(MatchEvent matchEvent)
    {
        _context.MatchEvents.Update(matchEvent);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(matchEvent.Id) ?? matchEvent;
    }

    public async Task DeleteAsync(Guid id)
    {
        var matchEvent = await _context.MatchEvents.FindAsync(id);
        if (matchEvent != null)
        {
            _context.MatchEvents.Remove(matchEvent);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.MatchEvents.AnyAsync(me => me.Id == id);
    }

    public async Task DeleteByMatchIdAsync(Guid matchId)
    {
        var events = await _context.MatchEvents
            .Where(me => me.MatchId == matchId)
            .ToListAsync();

        if (events.Any())
        {
            _context.MatchEvents.RemoveRange(events);
            await _context.SaveChangesAsync();
        }
    }
}