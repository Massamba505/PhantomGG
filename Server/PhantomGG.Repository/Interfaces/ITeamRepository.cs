using PhantomGG.Models.Entities;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;

namespace PhantomGG.Repository.Interfaces;

public interface ITeamRepository
{
    #region Team Query Operations
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetByUserAsync(Guid userId);
    Task<PaginatedResponse<Team>> SearchAsync(TeamSearchDto searchDto);
    #endregion

    #region Team CRUD Operations
    Task<Team> CreateAsync(Team team);
    Task<Team> UpdateAsync(Team team);
    Task DeleteAsync(Guid id);
    #endregion

    #region Team Tournament Management
    Task RegisterForTournamentAsync(Guid teamId, Guid tournamentId);
    Task UpdateTeamTournamentStatusAsync(Guid tournamentId, Guid teamId, string status);
    Task RemoveFromTournamentAsync(Guid tournamentId, Guid teamId);
    Task<IEnumerable<Tournament>> GetTournamentsByTeamAsync(Guid teamId);
    Task<int> GetTournamentTeamCountAsync(Guid tournamentId);
    #endregion

    #region Team Player Management
    Task<IEnumerable<Player>> GetTeamPlayersAsync(Guid teamId);
    Task<int> GetTeamPlayerCountAsync(Guid teamId);
    #endregion

    #region Validation Operations
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsTeamNameUniqueInTournamentAsync(string teamName, Guid tournamentId, Guid? excludeTeamId = null);
    Task<bool> IsUserTeamOwnerAsync(Guid teamId, Guid userId);
    #endregion
}
