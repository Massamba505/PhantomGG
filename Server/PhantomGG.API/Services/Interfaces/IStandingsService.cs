using PhantomGG.API.DTOs.TournamentStanding;

namespace PhantomGG.API.Services.Interfaces;

public interface IStandingsService
{
    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId);
}
