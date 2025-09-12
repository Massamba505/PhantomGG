using PhantomGG.Models.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface ITournamentFormatRepository
{
    Task<IEnumerable<TournamentFormat>> GetAllAsync();
    Task<TournamentFormat?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
