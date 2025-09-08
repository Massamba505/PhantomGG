using PhantomGG.API.Models;
using PhantomGG.API.DTOs.Match;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetAllAsync();
    Task<Match?> GetByIdAsync(Guid id);
    Task<IEnumerable<Match>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<Match>> GetByTeamAsync(Guid teamId);
    Task<IEnumerable<Match>> GetUpcomingMatchesAsync(Guid tournamentId);
    Task<IEnumerable<Match>> GetCompletedMatchesAsync(Guid tournamentId);
    Task<IEnumerable<Match>> SearchAsync(MatchSearchDto searchDto);
    Task<Match> CreateAsync(Match match);
    Task<Match> UpdateAsync(Match match);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> TeamsHaveMatchOnDateAsync(Guid homeTeamId, Guid awayTeamId, DateTime matchDate, Guid? excludeMatchId = null);
    Task<int> GetCompletedMatchCountAsync(Guid tournamentId);
    Task<int> GetTotalMatchCountAsync(Guid tournamentId);
}
