using PhantomGG.API.Models;
using PhantomGG.API.DTOs.Player;

namespace PhantomGG.API.Repositories.Interfaces;

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player?> GetByIdAsync(Guid id);
    Task<IEnumerable<Player>> GetByTeamAsync(Guid teamId);
    Task<IEnumerable<Player>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<Player>> SearchAsync(PlayerSearchDto searchDto);
    Task<Player> CreateAsync(Player player);
    Task<Player> UpdateAsync(Player player);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetPlayerCountByTeamAsync(Guid teamId);
}
