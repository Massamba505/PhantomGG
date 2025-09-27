using PhantomGG.Models.DTOs.TournamentStanding;

namespace PhantomGG.Repository.Interfaces;

public interface ITournamentStandingRepository
{
    Task<IEnumerable<TournamentStandingDto>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<PlayerGoalStandingDto>> GetPlayerGoalStandingsAsync(Guid tournamentId);
    Task<IEnumerable<PlayerAssistStandingDto>> GetPlayerAssistStandingsAsync(Guid tournamentId);
}
