using PhantomGG.Models.Entities;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Repository.Interfaces;

public interface ITournamentRepository
{
    #region Tournament Query Operations
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId);
    Task<IEnumerable<Tournament>> GetMyTournamentsAsync(Guid userId); // As organizer or participant
    #endregion

    #region Tournament Search & Filtering
    Task<PaginatedResponse<Tournament>> SearchAsync(TournamentSearchDto searchDto, Guid? organizerId = null);
    #endregion

    #region Tournament CRUD Operations
    Task<Tournament> CreateAsync(Tournament tournament);
    Task<Tournament> UpdateAsync(Tournament tournament);
    Task DeleteAsync(Guid id);
    #endregion

    #region Team Management Operations
    Task<IEnumerable<Team>> GetTournamentTeamsAsync(Guid tournamentId);
    Task<IEnumerable<Team>> GetPendingTeamsAsync(Guid tournamentId);
    Task<IEnumerable<Team>> GetApprovedTeamsAsync(Guid tournamentId);
    Task<bool> IsTeamRegisteredAsync(Guid tournamentId, Guid teamId);
    Task<TournamentTeam?> GetTeamRegistrationAsync(Guid tournamentId, Guid teamId);
    Task UpdateTeamRegistrationStatusAsync(Guid tournamentId, Guid teamId, string status, string? reason = null);
    #endregion

    #region Validation Operations
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsOrganizerAsync(Guid tournamentId, Guid userId);
    Task<int> GetApprovedTeamCountAsync(Guid tournamentId);
    #endregion
}
