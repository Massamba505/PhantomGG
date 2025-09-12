using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId);
    Task<IEnumerable<Tournament>> SearchAsync(TournamentSearchDto searchDto);
    Task<PaginatedResponse<Tournament>> SearchWithPaginationAsync(TournamentSearchDto searchDto, Guid? userId = null);
    Task<Tournament> CreateAsync(Tournament tournament);
    Task<Tournament> UpdateAsync(Tournament tournament);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsOrganizerAsync(Guid tournamentId, Guid userId);
    Task<int> GetTeamCountAsync(Guid tournamentId);
    Task<int> GetApprovedTeamCountAsync(Guid tournamentId);
}
