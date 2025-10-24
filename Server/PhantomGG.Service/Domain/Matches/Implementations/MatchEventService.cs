using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Mappings;
using PhantomGG.Common.Enums;
using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Service.Validation.Interfaces;
using PhantomGG.Service.Domain.Matches.Interfaces;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;

namespace PhantomGG.Service.Domain.Matches.Implementations;

public class MatchEventService(
    IMatchEventRepository matchEventRepository,
    IMatchRepository matchRepository,
    IMatchValidationService matchValidationService,
    ICacheInvalidationService cacheInvalidationService,
    HybridCache cache) : IMatchEventService
{
    private readonly IMatchEventRepository _matchEventRepository = matchEventRepository;
    private readonly IMatchRepository _matchRepository = matchRepository;
    private readonly IMatchValidationService _matchValidationService = matchValidationService;
    private readonly ICacheInvalidationService _cacheInvalidationService = cacheInvalidationService;
    private readonly HybridCache _cache = cache;

    public async Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(Guid matchId)
    {
        var events = await _matchEventRepository.GetByMatchIdAsync(matchId);
        return events.Select(e => e.ToDto());
    }

    public async Task<IEnumerable<MatchEventDto>> GetPlayerEventsAsync(Guid playerId)
    {
        var cacheKey = $"player_events_{playerId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var events = await _matchEventRepository.GetByPlayerIdAsync(playerId);
            return events.Select(e => e.ToDto());
        }, options);
    }

    public async Task<IEnumerable<MatchEventDto>> GetTeamEventsAsync(Guid teamId)
    {
        var cacheKey = $"team_events_{teamId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var events = await _matchEventRepository.GetByTeamIdAsync(teamId);
            return events.Select(e => e.ToDto());
        }, options);
    }

    public async Task<MatchEventDto> GetMatchEventByIdAsync(Guid id)
    {
        var cacheKey = $"match_event_{id}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var matchEvent = await _matchEventRepository.GetByIdAsync(id);
            if (matchEvent == null)
                throw new NotFoundException("Match event not found");

            return matchEvent.ToDto();
        }, options);
    }

    public async Task<MatchEventDto> CreateMatchEventAsync(CreateMatchEventDto createDto, Guid userId)
    {
        await _matchValidationService.ValidateCanUpdateMatchAsync(createDto.MatchId, userId);

        await _matchValidationService.ValidatePlayerTeamRelationshipAsync(createDto.PlayerId, createDto.TeamId, createDto.MatchId);

        var matchEvent = createDto.ToEntity();
        var createdEvent = await _matchEventRepository.CreateAsync(matchEvent);

        if (createDto.EventType == MatchEventType.Goal)
        {
            await RecalculateMatchScoresAsync(createDto.MatchId);
        }

        var matchEntity = await _matchRepository.GetByIdAsync(createDto.MatchId);
        if (matchEntity != null)
        {
            await _cacheInvalidationService.InvalidatePlayerStatsAsync(createDto.PlayerId, createDto.TeamId, matchEntity.TournamentId);
        }

        await _cacheInvalidationService.InvalidateMatchCacheAsync(createDto.MatchId);

        return createdEvent.ToDto();
    }

    public async Task<MatchEventDto> UpdateMatchEventAsync(Guid id, UpdateMatchEventDto updateDto, Guid userId)
    {
        var existingEvent = await _matchEventRepository.GetByIdAsync(id);
        if (existingEvent == null)
            throw new NotFoundException("Match event not found");

        await _matchValidationService.ValidateCanUpdateMatchAsync(existingEvent.MatchId, userId);

        var wasGoalEvent = existingEvent.EventType == (int)MatchEventType.Goal;
        var willBeGoalEvent = false;

        if (updateDto.EventType.HasValue)
        {
            existingEvent.EventType = (int)updateDto.EventType.Value;
            willBeGoalEvent = updateDto.EventType.Value == MatchEventType.Goal;
        }
        else
        {
            willBeGoalEvent = wasGoalEvent;
        }

        if (updateDto.Minute.HasValue)
            existingEvent.Minute = updateDto.Minute.Value;

        if (updateDto.PlayerId.HasValue)
        {
            await _matchValidationService.ValidatePlayerTeamRelationshipAsync(updateDto.PlayerId.Value, existingEvent.TeamId, existingEvent.MatchId);
            existingEvent.PlayerId = updateDto.PlayerId.Value;
        }

        var updatedEvent = await _matchEventRepository.UpdateAsync(existingEvent);

        if (wasGoalEvent || willBeGoalEvent)
        {
            await RecalculateMatchScoresAsync(existingEvent.MatchId);
        }

        var matchEntity = await _matchRepository.GetByIdAsync(existingEvent.MatchId);
        if (matchEntity != null)
        {
            await _cacheInvalidationService.InvalidatePlayerStatsAsync(existingEvent.PlayerId, existingEvent.TeamId, matchEntity.TournamentId);
        }

        await _cacheInvalidationService.InvalidateMatchCacheAsync(existingEvent.MatchId);

        return updatedEvent.ToDto();
    }

    public async Task DeleteMatchEventAsync(Guid id, Guid userId)
    {
        var existingEvent = await _matchEventRepository.GetByIdAsync(id);
        if (existingEvent == null)
            throw new NotFoundException("Match event not found");

        await _matchValidationService.ValidateCanUpdateMatchAsync(existingEvent.MatchId, userId);

        var wasGoalEvent = existingEvent.EventType == (int)MatchEventType.Goal;

        await _matchEventRepository.DeleteAsync(id);

        if (wasGoalEvent)
        {
            await RecalculateMatchScoresAsync(existingEvent.MatchId);
        }

        var matchEntity = await _matchRepository.GetByIdAsync(existingEvent.MatchId);
        if (matchEntity != null)
        {
            await _cacheInvalidationService.InvalidatePlayerStatsAsync(existingEvent.PlayerId, existingEvent.TeamId, matchEntity.TournamentId);
        }

        await _cacheInvalidationService.InvalidateMatchCacheAsync(existingEvent.MatchId);
    }

    private async Task RecalculateMatchScoresAsync(Guid matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null) return;

        var matchEvents = await _matchEventRepository.GetByMatchIdAsync(matchId);
        var goalEvents = matchEvents.Where(e => e.EventType == (int)MatchEventType.Goal);

        var homeScore = goalEvents.Count(e => e.TeamId == match.HomeTeamId);
        var awayScore = goalEvents.Count(e => e.TeamId == match.AwayTeamId);

        if (match.HomeScore != homeScore || match.AwayScore != awayScore)
        {
            match.HomeScore = homeScore;
            match.AwayScore = awayScore;
            await _matchRepository.UpdateAsync(match);
        }
    }
}
