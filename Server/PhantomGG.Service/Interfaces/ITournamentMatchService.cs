using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Match;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentMatchService
{
    Task<IEnumerable<MatchDto>> GetMatchesAsync(Guid tournamentId, MatchStatus? status);
    Task CreateBracketAsync(Guid tournamentId, Guid organizerId);
}
