using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Service.Interfaces;

public interface ITeamService
{
    #region Public Team Operations (from Controller)
    Task<PaginatedResponse<TeamDto>> SearchAsync(TeamSearchDto searchDto);
    Task<TeamDto> GetByIdAsync(Guid id);
    Task<IEnumerable<PlayerDto>> GetTeamPlayersAsync(Guid teamId);
    #endregion

    #region User Team Management (from Controller)
    Task<IEnumerable<TeamDto>> GetMyTeamsAsync(Guid userId);
    Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid userId);
    Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<string> UploadLogoAsync(Guid teamId, IFormFile file, Guid userId);
    #endregion

    #region User Player Management (from Controller)
    Task<PlayerDto> AddPlayerToTeamAsync(Guid teamId, object playerDto, Guid userId);
    Task<PlayerDto> UpdateTeamPlayerAsync(Guid teamId, Guid playerId, UpdatePlayerDto updateDto, Guid userId);
    Task RemovePlayerFromTeamAsync(Guid teamId, Guid playerId, Guid userId);
    Task<string> UploadPlayerPhotoAsync(Guid teamId, Guid playerId, IFormFile file, Guid userId);
    #endregion
}
