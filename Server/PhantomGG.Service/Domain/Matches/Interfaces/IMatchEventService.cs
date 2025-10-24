using PhantomGG.Models.DTOs.MatchEvent;

namespace PhantomGG.Service.Domain.Matches.Interfaces;

public interface IMatchEventService
{
    Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(Guid matchId);
    Task<IEnumerable<MatchEventDto>> GetPlayerEventsAsync(Guid playerId);
    Task<IEnumerable<MatchEventDto>> GetTeamEventsAsync(Guid teamId);
    Task<MatchEventDto> GetMatchEventByIdAsync(Guid id);
    Task<MatchEventDto> CreateMatchEventAsync(CreateMatchEventDto createDto, Guid userId);
    Task<MatchEventDto> UpdateMatchEventAsync(Guid id, UpdateMatchEventDto updateDto, Guid userId);
    Task DeleteMatchEventAsync(Guid id, Guid userId);
}
