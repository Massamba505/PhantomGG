using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.MatchEvent;

namespace PhantomGG.Service.Interfaces;

public interface IMatchService
{
    Task<MatchDto> GetByIdAsync(Guid id);
    Task<IEnumerable<MatchDto>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<MatchDto>> GetByTeamAsync(Guid teamId);
    Task<IEnumerable<MatchDto>> GetUpcomingMatchesAsync(Guid tournamentId);
    Task<IEnumerable<MatchDto>> GetCompletedMatchesAsync(Guid tournamentId);
    Task<IEnumerable<MatchDto>> SearchAsync(MatchSearchDto searchDto);

    Task<MatchDto> CreateAsync(CreateMatchDto createDto, Guid userId);
    Task<MatchDto> UpdateAsync(Guid id, UpdateMatchDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);

    Task<MatchDto> UpdateResultAsync(Guid matchId, MatchResultDto resultDto, Guid organizerId);

    Task<IEnumerable<MatchDto>> GenerateRoundRobinFixturesAsync(GenerateFixturesDto generateDto, Guid userId);
    Task<FixtureGenerationStatusDto> GetFixtureGenerationStatusAsync(Guid tournamentId);

    Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(Guid matchId);
    Task<IEnumerable<MatchEventDto>> GetPlayerEventsAsync(Guid playerId);
    Task<IEnumerable<MatchEventDto>> GetTeamEventsAsync(Guid teamId);
    Task<MatchEventDto> GetMatchEventByIdAsync(Guid id);
    Task<MatchEventDto> CreateMatchEventAsync(CreateMatchEventDto createDto, Guid userId);
    Task<MatchEventDto> UpdateMatchEventAsync(Guid id, UpdateMatchEventDto updateDto, Guid userId);
    Task DeleteMatchEventAsync(Guid id, Guid userId);
}
