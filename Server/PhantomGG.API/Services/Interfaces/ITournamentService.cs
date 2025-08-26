using PhantomGG.API.DTOs.Tournament;

namespace PhantomGG.API.Services.Interfaces;

public interface ITournamentService
{
    Task<IEnumerable<TournamentDto>> GetAllAsync();
    Task<TournamentDto> GetByIdAsync(Guid id);
    Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(Guid organizerId);
    Task<IEnumerable<TournamentDto>> SearchAsync(TournamentSearchDto searchDto);
    Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId);
    Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
}
