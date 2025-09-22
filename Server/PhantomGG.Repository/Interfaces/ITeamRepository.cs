using PhantomGG.Models.Entities;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;

namespace PhantomGG.Repository.Interfaces;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetByUserAsync(Guid userId);
    Task<Team> CreateAsync(Team team);
    Task<Team> UpdateAsync(Team team);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Team>> SearchAsync(TeamSearchDto searchDto, Guid? userId = null);
    Task<bool> IsTeamNameUniqueInTournamentAsync(string teamName, Guid tournamentId, Guid? excludeTeamId = null);
}
