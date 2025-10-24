using PhantomGG.Models.DTOs.TournamentStanding;

namespace PhantomGG.Service.Domain.Tournaments.Interfaces;

public interface ITournamentStandingService
{
    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId, Guid? currentUserId = null);
    Task<IEnumerable<PlayerGoalStandingDto>> GetPlayerGoalStandingsAsync(Guid tournamentId, Guid? currentUserId = null);
    Task<IEnumerable<PlayerAssistStandingDto>> GetPlayerAssistStandingsAsync(Guid tournamentId, Guid? currentUserId = null);
}
