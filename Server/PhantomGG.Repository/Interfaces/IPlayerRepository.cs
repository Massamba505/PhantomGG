using PhantomGG.Models.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id);
    Task<IEnumerable<Player>> GetByTeamAsync(Guid teamId);
    Task<Player> CreateAsync(Player player);
    Task<Player> UpdateAsync(Player player);
    Task DeleteAsync(Guid id);
    Task<int> GetPlayerCountByTeamAsync(Guid teamId);
}
