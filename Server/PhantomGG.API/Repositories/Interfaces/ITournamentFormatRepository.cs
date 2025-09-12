using PhantomGG.API.Models;

namespace PhantomGG.API.Repositories.Interfaces;

public interface ITournamentFormatRepository
{
    Task<IEnumerable<TournamentFormat>> GetAllAsync();
    Task<TournamentFormat?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
