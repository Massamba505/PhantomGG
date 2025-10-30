using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Validation.Interfaces;

public interface IMatchValidationService
{
    Task<Match> ValidateMatchExistsAsync(Guid matchId);
    Task<Match> ValidateCanUpdateMatchAsync(Guid matchId, Guid userId);
    Task<Match> ValidateCanDeleteMatchAsync(Guid matchId, Guid userId);
    Task<Match> ValidateCanUpdateResultAsync(Guid matchId, Guid userId);
    Task ValidateTeamsCanPlayAsync(Guid homeTeamId, Guid awayTeamId, Guid tournamentId);
    Task ValidateMatchSchedulingAsync(Guid homeTeamId, Guid awayTeamId, DateTime matchDate, Guid? excludeMatchId = null);
    Task<Tournament> ValidateTournamentForMatchAsync(Guid tournamentId, Guid userId);
    Task ValidatePlayerTeamRelationshipAsync(Guid playerId, Guid teamId, Guid matchId);
}
