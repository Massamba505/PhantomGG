using PhantomGG.API.Models;
using PhantomGG.API.DTOs.Tournament;

namespace PhantomGG.API.Repositories.Interfaces;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId);
    Task<IEnumerable<Tournament>> SearchAsync(TournamentSearchDto searchDto);
    Task<Tournament> CreateAsync(Tournament tournament);
    Task<Tournament> UpdateAsync(Tournament tournament);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsOrganizerAsync(Guid tournamentId, Guid userId);
    Task<int> GetTeamCountAsync(Guid tournamentId);
}
