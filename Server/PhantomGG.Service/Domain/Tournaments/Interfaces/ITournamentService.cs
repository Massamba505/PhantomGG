using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Service.Domain.Tournaments.Interfaces;

public interface ITournamentService
{
    Task<PagedResult<TournamentDto>> SearchAsync(TournamentQuery query, Guid? userId = null);
    Task<PagedResult<TournamentDto>> GetUserTournamentsAsync(TournamentQuery query, Guid userId);
    Task<TournamentDto> GetByIdAsync(Guid id, Guid? currentUserId = null);
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
