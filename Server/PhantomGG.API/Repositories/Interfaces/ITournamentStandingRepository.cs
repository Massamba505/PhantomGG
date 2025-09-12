using PhantomGG.API.DTOs.TournamentStanding;

namespace PhantomGG.API.Repositories.Interfaces;

public interface ITournamentStandingRepository
{
    Task<IEnumerable<TournamentStandingDto>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<PlayerGoalStandingDto>> GetPlayerGoalStandingsAsync(Guid tournamentId);
    Task<IEnumerable<PlayerAssistStandingDto>> GetPlayerAssistStandingsAsync(Guid tournamentId);
}
