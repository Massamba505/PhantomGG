using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentValidationService
{
    Task<Tournament> ValidateTournamentExistsAsync(Guid tournamentId);
    Task<Tournament> ValidateCanUpdateAsync(Guid tournamentId, Guid userId);
    Task<Tournament> ValidateCanDeleteAsync(Guid tournamentId, Guid userId);
    Task<Tournament> ValidateCanManageTeamsAsync(Guid tournamentId, Guid userId);
    Task<Tournament> ValidateCanManageMatchesAsync(Guid tournamentId, Guid userId);
    Task<Tournament> ValidateCanManageTournamentAsync(Guid tournamentId, Guid userId);
    Task<Tournament> ValidateTeamCanRegisterAsync(Guid tournamentId);
    Task<Tournament> ValidateCanUpdateStatusAsync(Guid tournamentId, Guid userId, TournamentStatus newStatus);
}

public interface ITeamValidationService
{
    Task<Team> ValidateTeamExistsAsync(Guid userId);
    Task<Team> ValidateCanManageTeamAsync(Guid userId, Guid teamId);
    Task<Team> ValidateTeamCanBeDeleted(Guid teamId, Guid userId);
    Task ValidateUserTeamNameUniqueness(string teamName, Guid managerId);
}

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