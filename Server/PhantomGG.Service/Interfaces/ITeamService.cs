using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.Entities;

namespace PhantomGG.Service.Interfaces;

public interface ITeamService
{
    Task<PaginatedResponse<TeamDto>> SearchAsync(TeamSearchDto searchDto, Guid? userId = null);
    Task<TeamDto> GetByIdAsync(Guid id);
    Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid userId);
    Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<IEnumerable<PlayerDto>> GetTeamPlayersAsync(Guid teamId);
    Task<PlayerDto> AddPlayerToTeamAsync(Guid teamId, CreatePlayerDto playerDto, Guid userId);
    Task<PlayerDto> UpdateTeamPlayerAsync(Guid teamId, Guid playerId, UpdatePlayerDto updateDto, Guid userId);
    Task RemovePlayerFromTeamAsync(Guid teamId, Guid playerId, Guid userId);
    Task<string> UploadLogoAsync(Team team, IFormFile file);
}
