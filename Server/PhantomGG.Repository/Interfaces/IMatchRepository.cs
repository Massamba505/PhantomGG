using PhantomGG.Models.DTOs;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Specifications;

namespace PhantomGG.Repository.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id);
    Task<IEnumerable<Match>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<Match>> GetByTournamentAndStatusAsync(Guid tournamentId, string status);
    Task<PagedResult<Match>> SearchAsync(MatchSpecification specification);
    Task<Match> CreateAsync(Match match);
    Task<Match> UpdateAsync(Match match);
    Task DeleteAsync(Guid id);
    Task<bool> TeamsHaveMatchOnDateAsync(Guid homeTeamId, Guid awayTeamId, DateTime matchDate, Guid? excludeMatchId = null);
}
