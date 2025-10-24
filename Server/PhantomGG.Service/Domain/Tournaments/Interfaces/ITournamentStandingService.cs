using PhantomGG.Models.DTOs.TournamentStanding;

namespace PhantomGG.Service.Domain.Tournaments.Interfaces;

public interface ITournamentStandingService
{
    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId);
    Task<IEnumerable<PlayerGoalStandingDto>> GetPlayerGoalStandingsAsync(Guid tournamentId);
    Task<IEnumerable<PlayerAssistStandingDto>> GetPlayerAssistStandingsAsync(Guid tournamentId);
}
