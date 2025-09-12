using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs.Team;

namespace PhantomGG.Service.Interfaces;

public interface ITeamService
{
    Task<IEnumerable<TeamDto>> GetAllAsync();
    Task<TeamDto> GetByIdAsync(Guid id);
    Task<IEnumerable<TeamDto>> GetByLeaderAsync(Guid leaderId);
    Task<IEnumerable<TeamDto>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<TeamDto>> GetByRegistrationStatusAsync(Guid tournamentId, string status);
    Task<IEnumerable<TeamDto>> SearchAsync(TeamSearchDto searchDto);
    Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid leaderId);
    Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task ApproveTeamAsync(Guid teamId, Guid organizerId);
    Task RejectTeamAsync(Guid teamId, Guid organizerId, string? reason = null);
    Task<string> UploadTeamLogoAsync(Guid teamId, IFormFile file, Guid userId);
}
