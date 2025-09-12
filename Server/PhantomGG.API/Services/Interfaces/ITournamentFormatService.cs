using PhantomGG.API.DTOs.TournamentFormat;

namespace PhantomGG.API.Services.Interfaces;

public interface ITournamentFormatService
{
    Task<IEnumerable<TournamentFormatDto>> GetAllActiveAsync();
}
