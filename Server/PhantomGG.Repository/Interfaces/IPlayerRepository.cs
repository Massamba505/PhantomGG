using PhantomGG.Models.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface IPlayerRepository
{
    #region Basic Player Operations
    Task<Player?> GetByIdAsync(Guid id);
    Task<IEnumerable<Player>> GetByTeamAsync(Guid teamId);
    #endregion

    #region Player CRUD Operations  
    Task<Player> CreateAsync(Player player);
    Task<Player> UpdateAsync(Player player);
    Task DeleteAsync(Guid id);
    #endregion

    #region Validation Operations
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetPlayerCountByTeamAsync(Guid teamId);
    #endregion
}
