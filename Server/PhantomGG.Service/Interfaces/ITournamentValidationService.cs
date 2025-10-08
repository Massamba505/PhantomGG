using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentValidationService
{
    Task<Tournament> ValidateTournamentExistsAsync(Guid tournamentId);
    Task<Tournament> ValidateCanUpdateAsync(Guid tournamentId, Guid userId);
    Task<Tournament> ValidateCanDeleteAsync(Guid tournamentId, Guid userId);
    Task<Tournament> ValidateCanManageTeamsAsync(Guid tournamentId, Guid userId);
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