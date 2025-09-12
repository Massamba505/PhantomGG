using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.MatchEvent;

namespace PhantomGG.Service.Interfaces;

public interface IMatchService
{
    Task<IEnumerable<MatchDto>> GetAllAsync();
    Task<MatchDto> GetByIdAsync(Guid id);
    Task<IEnumerable<MatchDto>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<MatchDto>> GetByTeamAsync(Guid teamId);
    Task<IEnumerable<MatchDto>> GetUpcomingMatchesAsync(Guid tournamentId);
    Task<IEnumerable<MatchDto>> GetCompletedMatchesAsync(Guid tournamentId);
    Task<IEnumerable<MatchDto>> SearchAsync(MatchSearchDto searchDto);
    Task<MatchDto> CreateAsync(CreateMatchDto createDto, Guid userId);
    Task<MatchDto> UpdateAsync(Guid id, UpdateMatchDto updateDto, Guid userId);
    Task<MatchDto> UpdateResultAsync(Guid id, MatchResultDto resultDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<bool> IsMatchOwnedByUserAsync(Guid matchId, Guid userId);

    // Fixture generation
    Task<IEnumerable<MatchDto>> GenerateRoundRobinFixturesAsync(GenerateFixturesDto generateDto, Guid userId);
    Task<IEnumerable<MatchDto>> GenerateKnockoutFixturesAsync(GenerateFixturesDto generateDto, Guid userId);
    Task<IEnumerable<MatchDto>> AutoGenerateFixturesAsync(AutoGenerateFixturesDto generateDto, Guid userId);
    Task<FixtureGenerationStatusDto> GetFixtureGenerationStatusAsync(Guid tournamentId);

    // Match status management
    Task<MatchDto> StartMatchAsync(Guid matchId, Guid userId);
    Task<MatchDto> EndMatchAsync(Guid matchId, Guid userId);
    Task<MatchDto> CancelMatchAsync(Guid matchId, string reason, Guid userId);
    Task<MatchDto> PostponeMatchAsync(Guid matchId, DateTime newDate, string reason, Guid userId);

    // Match events
    Task AddMatchEventAsync(Guid matchId, CreateMatchEventDto eventDto, Guid userId);
    Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(Guid matchId);
}
