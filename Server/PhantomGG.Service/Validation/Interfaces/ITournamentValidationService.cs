using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Validation.Interfaces;

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
