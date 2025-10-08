using PhantomGG.Service.Interfaces;
using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Mappings;
using Microsoft.Extensions.Caching.Hybrid;

namespace PhantomGG.Service.Implementations;

public class MatchEventService(
    IMatchEventRepository matchEventRepository,
    IPlayerRepository playerRepository,
    IMatchValidationService matchValidationService,
    ICacheInvalidationService cacheInvalidationService,
    HybridCache cache) : IMatchEventService
{
    private readonly IMatchEventRepository _matchEventRepository = matchEventRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IMatchValidationService _matchValidationService = matchValidationService;
    private readonly ICacheInvalidationService _cacheInvalidationService = cacheInvalidationService;
    private readonly HybridCache _cache = cache;

    public async Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(Guid matchId)
    {
        var cacheKey = $"match_events_{matchId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var events = await _matchEventRepository.GetByMatchIdAsync(matchId);
            return events.Select(e => e.ToDto());
        }, options);
    }

    public async Task<IEnumerable<MatchEventDto>> GetPlayerEventsAsync(Guid playerId)
    {
        var cacheKey = $"player_events_{playerId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
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
            Expiration = TimeSpan.FromMinutes(5)
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

        // Validate player belongs to one of the teams in the match
        await _matchValidationService.ValidatePlayerTeamRelationshipAsync(createDto.PlayerId, createDto.TeamId, createDto.MatchId);

        var matchEvent = createDto.ToEntity();
        var createdEvent = await _matchEventRepository.CreateAsync(matchEvent);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(createDto.MatchId);

        return createdEvent.ToDto();
    }

    public async Task<MatchEventDto> UpdateMatchEventAsync(Guid id, UpdateMatchEventDto updateDto, Guid userId)
    {
        var existingEvent = await _matchEventRepository.GetByIdAsync(id);
        if (existingEvent == null)
            throw new NotFoundException("Match event not found");

        await _matchValidationService.ValidateCanUpdateMatchAsync(existingEvent.MatchId, userId);

        if (updateDto.EventType.HasValue)
            existingEvent.EventType = updateDto.EventType.Value.ToString();

        if (updateDto.Minute.HasValue)
            existingEvent.Minute = updateDto.Minute.Value;

        if (updateDto.PlayerId.HasValue)
        {
            await _matchValidationService.ValidatePlayerTeamRelationshipAsync(updateDto.PlayerId.Value, existingEvent.TeamId, existingEvent.MatchId);
            existingEvent.PlayerId = updateDto.PlayerId.Value;
        }

        var updatedEvent = await _matchEventRepository.UpdateAsync(existingEvent);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(existingEvent.MatchId);

        return updatedEvent.ToDto();
    }

    public async Task DeleteMatchEventAsync(Guid id, Guid userId)
    {
        var existingEvent = await _matchEventRepository.GetByIdAsync(id);
        if (existingEvent == null)
            throw new NotFoundException("Match event not found");

        await _matchValidationService.ValidateCanUpdateMatchAsync(existingEvent.MatchId, userId);

        await _matchEventRepository.DeleteAsync(id);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(existingEvent.MatchId);
    }
}
