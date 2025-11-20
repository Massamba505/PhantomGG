using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Team;

namespace PhantomGG.Service.Domain.Tournaments.Interfaces;

public interface ITournamentTeamService
{
    Task<IEnumerable<TournamentTeamDto>> GetTeamsAsync(Guid tournamentId, TeamRegistrationStatus? status = null);
    Task<IEnumerable<TournamentTeamDto>> GetOrganizerPendingApprovalsAsync(Guid organizerId);
    Task<bool> IsTeamRegisteredAsync(Guid tournamentId, Guid teamId);
    Task RegisterTeamAsync(Guid tournamentId, Guid teamId, Guid userId);
    Task ManageTeamAsync(Guid tournamentId, Guid teamId, TeamAction action, Guid userId);
    Task RemoveTeamAsync(Guid tournamentId, Guid teamId, Guid userId);
    Task<int> GetTeamCountAsync(Guid tournamentId, TeamRegistrationStatus status);
}
