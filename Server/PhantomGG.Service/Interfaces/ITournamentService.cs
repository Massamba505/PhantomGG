using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentService
{
    Task<PagedResult<TournamentDto>> SearchAsync(TournamentQuery query, Guid? userId = null);
    Task<TournamentDto> GetByIdAsync(Guid id);
    Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId);
    Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid organizerId);
    Task DeleteAsync(Guid id, Guid organizerId);
    Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(Guid organizerId);
    Task UpdateTournamentStatusesAsync();
}

public interface ITournamentBackgroundJobService
{
    Task UpdateTournamentStatusesAsync();
}
