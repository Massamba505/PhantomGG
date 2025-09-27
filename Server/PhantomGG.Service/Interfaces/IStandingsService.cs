using PhantomGG.Models.DTOs.TournamentStanding;

namespace PhantomGG.Service.Interfaces;

public interface IStandingsService
{
    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId);
}
