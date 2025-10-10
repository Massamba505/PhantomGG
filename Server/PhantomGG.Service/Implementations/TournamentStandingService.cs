using PhantomGG.Models.DTOs.TournamentStanding;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

namespace PhantomGG.Service.Implementations;

public class TournamentStandingService(
    ITournamentStandingRepository tournamentStandingRepository,
    ITournamentValidationService validationService,
    HybridCache cache) : ITournamentStandingService
{
    private readonly ITournamentStandingRepository _tournamentStandingRepository = tournamentStandingRepository;
    private readonly ITournamentValidationService _validationService = validationService;
    private readonly HybridCache _cache = cache;

    public async Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId)
    {
        await _validationService.ValidateTournamentExistsAsync(tournamentId);

        var cacheKey = $"tournament_standings_{tournamentId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(20)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            return await _tournamentStandingRepository.GetByTournamentAsync(tournamentId);
        }, options);
    }

    public async Task<IEnumerable<PlayerGoalStandingDto>> GetPlayerGoalStandingsAsync(Guid tournamentId)
    {
        await _validationService.ValidateTournamentExistsAsync(tournamentId);

        var cacheKey = $"player_goal_standings_{tournamentId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(15)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            return await _tournamentStandingRepository.GetPlayerGoalStandingsAsync(tournamentId);
        }, options);
    }

    public async Task<IEnumerable<PlayerAssistStandingDto>> GetPlayerAssistStandingsAsync(Guid tournamentId)
    {
        await _validationService.ValidateTournamentExistsAsync(tournamentId);

        var cacheKey = $"player_assist_standings_{tournamentId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(15)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            return await _tournamentStandingRepository.GetPlayerAssistStandingsAsync(tournamentId);
        }, options);
    }
}
