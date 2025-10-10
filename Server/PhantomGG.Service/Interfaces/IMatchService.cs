using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs;
using PhantomGG.Common.Enums;

namespace PhantomGG.Service.Interfaces;

public interface IMatchService
{
    Task<MatchDto> GetByIdAsync(Guid id);
    Task<IEnumerable<MatchDto>> GetByTournamentAndStatusAsync(Guid tournamentId, MatchStatus? status);
    Task<PagedResult<MatchDto>> SearchAsync(MatchQuery query);
    Task<MatchDto> CreateAsync(CreateMatchDto createDto, Guid userId);
    Task<MatchDto> UpdateAsync(Guid id, UpdateMatchDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<MatchDto> UpdateResultAsync(Guid matchId, MatchResultDto resultDto, Guid organizerId);
    Task CreateTournamentBracketAsync(Guid tournamentId, Guid organizerId);
}
