using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId);
    Task<IEnumerable<Tournament>> GetMyTournamentsAsync(Guid userId);
    Task<PaginatedResult<Tournament>> SearchAsync(TournamentSearchDto searchDto, Guid? organizerId = null);
    Task<IEnumerable<Tournament>> GetTournamentsByTeamAsync(Guid teamId);
    Task<Tournament> CreateAsync(Tournament tournament);
    Task<Tournament> UpdateAsync(Tournament tournament);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<TournamentTeam>> GetTournamentTeamsAsync(Guid tournamentId);
    Task<IEnumerable<TournamentTeam>> GetPendingTeamsAsync(Guid tournamentId);
    Task<bool> IsTeamRegisteredAsync(Guid tournamentId, Guid teamId);
    Task<TournamentTeam?> GetTeamRegistrationAsync(Guid tournamentId, Guid teamId);
    Task<int> GetTournamentTeamCountAsync(Guid tournamentId);
    Task RegisterTeamForTournamentAsync(TournamentTeam tournamentTeam);
    Task ApproveTeamAsync(Guid tournamentId, Guid teamId);
    Task RejectTeamAsync(Guid tournamentId, Guid teamId);
    Task RemoveTeamFromTournamentAsync(Guid tournamentId, Guid teamId);
    Task<int> GetApprovedTeamCountAsync(Guid tournamentId);
    Task<IEnumerable<Match>> GetTournamentMatchesAsync(Guid tournamentId);
    Task<IEnumerable<MatchEvent>> GetTournamentMatchEventsAsync(Guid tournamentId);
}
