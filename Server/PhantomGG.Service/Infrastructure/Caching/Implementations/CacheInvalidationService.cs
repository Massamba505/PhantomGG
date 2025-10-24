using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
namespace PhantomGG.Service.Infrastructure.Caching.Implementations;

public class CacheInvalidationService(HybridCache cache) : ICacheInvalidationService
{
    private readonly HybridCache _cache = cache;

    public async Task InvalidateUserCacheAsync(Guid userId)
    {
        await _cache.RemoveAsync($"user_{userId}");
    }

    public async Task InvalidateTournamentCacheAsync(Guid tournamentId)
    {
        await _cache.RemoveAsync($"tournament_{tournamentId}");
        await _cache.RemoveAsync($"tournament_teams_{tournamentId}");
        await _cache.RemoveAsync($"tournament_teams_{tournamentId}_all");
        await _cache.RemoveAsync($"tournament_teams_{tournamentId}_Pending");
        await _cache.RemoveAsync($"tournament_teams_{tournamentId}_Approved");
        await _cache.RemoveAsync($"tournament_teams_{tournamentId}_Rejected");
        await _cache.RemoveAsync($"tournament_matches_{tournamentId}");
        await _cache.RemoveAsync($"upcoming_matches_{tournamentId}");
        await _cache.RemoveAsync($"completed_matches_{tournamentId}");
        await _cache.RemoveAsync($"fixture_status_{tournamentId}");
        await _cache.RemoveAsync($"tournament_standings_{tournamentId}");
        await _cache.RemoveAsync($"player_goal_standings_{tournamentId}");
        await _cache.RemoveAsync($"player_assist_standings_{tournamentId}");
    }

    public async Task InvalidateTeamCacheAsync(Guid teamId)
    {
        await _cache.RemoveAsync($"team_{teamId}");
        await _cache.RemoveAsync($"team_players_{teamId}");
    }

    public async Task InvalidateMatchCacheAsync(Guid matchId)
    {
        await _cache.RemoveAsync($"match_{matchId}");
    }

    public async Task InvalidateTournamentRelatedCacheAsync(Guid tournamentId)
    {
        await InvalidateTournamentCacheAsync(tournamentId);
        await _cache.RemoveAsync("all_tournaments");
        await _cache.RemoveAsync($"tournament_standings_{tournamentId}");
        await _cache.RemoveAsync($"player_goal_standings_{tournamentId}");
        await _cache.RemoveAsync($"player_assist_standings_{tournamentId}");
    }

    public async Task InvalidateTeamRelatedCacheAsync(Guid teamId)
    {
        await InvalidateTeamCacheAsync(teamId);
        await _cache.RemoveAsync("all_teams");
    }

    public async Task InvalidatePlayerStatsAsync(Guid playerId, Guid teamId, Guid tournamentId)
    {
        await _cache.RemoveAsync($"player_events_{playerId}");
        await _cache.RemoveAsync($"team_events_{teamId}");
        await _cache.RemoveAsync($"tournament_standings_{tournamentId}");
        await _cache.RemoveAsync($"player_goal_standings_{tournamentId}");
        await _cache.RemoveAsync($"player_assist_standings_{tournamentId}");
    }
}