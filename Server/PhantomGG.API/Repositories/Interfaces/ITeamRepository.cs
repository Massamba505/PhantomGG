using PhantomGG.API.Models;
using PhantomGG.API.DTOs.Team;

namespace PhantomGG.API.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetByManagerAsync(string manager);
    Task<IEnumerable<Team>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<Team>> SearchAsync(TeamSearchDto searchDto);
    Task<Team> CreateAsync(Team team);
    Task<Team> UpdateAsync(Team team);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> TeamNameExistsInTournamentAsync(string name, Guid tournamentId, Guid? excludeId = null);
}
