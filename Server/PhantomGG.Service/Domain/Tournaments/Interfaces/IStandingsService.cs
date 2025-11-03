using PhantomGG.Models.DTOs.TournamentStanding;

namespace PhantomGG.Service.Domain.Tournaments.Interfaces;

public interface IStandingsService
{
    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId);
}
