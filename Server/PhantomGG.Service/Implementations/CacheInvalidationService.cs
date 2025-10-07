using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.Service.Implementations;

public class CacheInvalidationService : ICacheInvalidationService
{
    private readonly HybridCache _cache;

    public CacheInvalidationService(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task InvalidateUserCacheAsync(Guid userId)
    {
        await _cache.RemoveAsync($"user_{userId}");
    }

    public async Task InvalidateTournamentCacheAsync(Guid tournamentId)
    {
        await _cache.RemoveAsync($"tournament_{tournamentId}");
        await _cache.RemoveAsync($"tournament_teams_{tournamentId}");
        await _cache.RemoveAsync($"tournament_matches_{tournamentId}");
        await _cache.RemoveAsync($"upcoming_matches_{tournamentId}");
        await _cache.RemoveAsync($"completed_matches_{tournamentId}");
        await _cache.RemoveAsync($"fixture_status_{tournamentId}");

    }

    public async Task InvalidateTeamCacheAsync(Guid teamId)
    {
        await _cache.RemoveAsync($"team_{teamId}");
        await _cache.RemoveAsync($"team_players_{teamId}");
    }

    public async Task InvalidateMatchCacheAsync(Guid matchId)
    {
        await _cache.RemoveAsync($"match_{matchId}");
        await _cache.RemoveAsync($"match_events_{matchId}");
    }

    public async Task InvalidateTournamentRelatedCacheAsync(Guid tournamentId)
    {
        await InvalidateTournamentCacheAsync(tournamentId);
        await _cache.RemoveAsync("all_tournaments");
    }

    public async Task InvalidateTeamRelatedCacheAsync(Guid teamId)
    {
        await InvalidateTeamCacheAsync(teamId);
        await _cache.RemoveAsync("all_teams");
    }
}