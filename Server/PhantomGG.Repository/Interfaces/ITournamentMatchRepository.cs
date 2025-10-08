using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface ITournamentMatchRepository
{
    Task<IEnumerable<Match>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<Match>> GetByTournamentAndStatusAsync(Guid tournamentId, string status);
    Task<IEnumerable<MatchEvent>> GetEventsByTournamentAsync(Guid tournamentId);
}
