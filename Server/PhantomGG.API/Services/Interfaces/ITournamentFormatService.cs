using PhantomGG.API.DTOs.TournamentFormat;

namespace PhantomGG.API.Services.Interfaces;

public interface ITournamentFormatService
{
    Task<IEnumerable<TournamentFormatDto>> GetAllActiveAsync();
    Task<TournamentFormatDto> GetByIdAsync(Guid id);
    Task<TournamentFormatDto> CreateAsync(TournamentFormatDto formatDto);
    Task<TournamentFormatDto> UpdateAsync(Guid id, TournamentFormatDto formatDto);
    Task DeleteAsync(Guid id);
}
