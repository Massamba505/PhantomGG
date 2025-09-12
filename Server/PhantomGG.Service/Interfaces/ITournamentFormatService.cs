using PhantomGG.Models.DTOs.TournamentFormat;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentFormatService
{
    Task<IEnumerable<TournamentFormatDto>> GetAllActiveAsync();
}
