using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Validation.Interfaces;

public interface IMatchEventValidationService
{
    Task<MatchEvent> ValidateMatchEventExistsAsync(Guid eventId);
    Task<MatchEvent> ValidateCanUpdateEventAsync(Guid eventId, Guid userId);
    Task<MatchEvent> ValidateCanDeleteEventAsync(Guid eventId, Guid userId);
    Task ValidateEventTimeAsync(int minute, Guid matchId);
    Task ValidatePlayerInMatchAsync(Guid playerId, Guid matchId);
    Task ValidatePlayerTeamInMatchAsync(Guid playerId, Guid teamId, Guid matchId);
    Task ValidateRedCardRulesAsync(Guid playerId, Guid matchId);
    Task ValidateYellowCardRulesAsync(Guid playerId, Guid matchId);
    Task ValidateGoalScorerAsync(Guid playerId, int eventType);
    Task ValidateEventTypeForMatchStatusAsync(int eventType, Guid matchId);
}
