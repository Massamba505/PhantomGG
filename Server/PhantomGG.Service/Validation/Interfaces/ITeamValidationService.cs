using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Validation.Interfaces;

public interface ITeamValidationService
{
    Task<Team> ValidateTeamExistsAsync(Guid userId);
    Task<Team> ValidateCanManageTeamAsync(Guid userId, Guid teamId);
    Task<Team> ValidateTeamCanBeDeleted(Guid teamId, Guid userId);
    Task ValidateUserTeamNameUniqueness(string teamName, Guid managerId);
    Task ValidateMinimumPlayersForTournamentAsync(Guid teamId, int minPlayers = 5);
    Task ValidateTeamHasRequiredPositionsAsync(Guid teamId);
}

