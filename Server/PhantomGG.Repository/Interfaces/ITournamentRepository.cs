using PhantomGG.Models.DTOs;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Specifications;

namespace PhantomGG.Repository.Interfaces;

public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<Tournament> CreateAsync(Tournament tournament);
    Task<Tournament> UpdateAsync(Tournament tournament);
    Task DeleteAsync(Guid id);
    Task<PagedResult<Tournament>> SearchAsync(TournamentSpecification spec);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId);
}
