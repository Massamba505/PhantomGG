using PhantomGG.Repository.Entities;
using PhantomGG.Models.DTOs;
using PhantomGG.Repository.Specifications;

namespace PhantomGG.Repository.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetByUserAsync(Guid userId);
    Task<Team> CreateAsync(Team team);
    Task<Team> UpdateAsync(Team team);
    Task DeleteAsync(Guid id);
    Task<PagedResult<Team>> SearchAsync(TeamSpecification specification);
    Task<bool> IsTeamNameUniqueInTournamentAsync(string teamName, Guid tournamentId, Guid? excludeTeamId = null);
}
