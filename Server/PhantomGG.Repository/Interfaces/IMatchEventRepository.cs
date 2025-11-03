using PhantomGG.Repository.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface IMatchEventRepository
{
    Task<IEnumerable<MatchEvent>> GetByMatchIdAsync(Guid matchId);
    Task<IEnumerable<MatchEvent>> GetByPlayerIdAsync(Guid playerId);
    Task<IEnumerable<MatchEvent>> GetByTeamIdAsync(Guid teamId);
    Task<MatchEvent?> GetByIdAsync(Guid id);
    Task<MatchEvent> CreateAsync(MatchEvent matchEvent);
    Task<MatchEvent> UpdateAsync(MatchEvent matchEvent);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task DeleteByMatchIdAsync(Guid matchId);
}