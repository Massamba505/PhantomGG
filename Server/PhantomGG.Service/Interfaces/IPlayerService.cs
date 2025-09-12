using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Service.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAllAsync();
    Task<PlayerDto> GetByIdAsync(Guid id);
    Task<IEnumerable<PlayerDto>> GetByTeamAsync(Guid teamId);
    Task<IEnumerable<PlayerDto>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<PlayerDto>> SearchAsync(PlayerSearchDto searchDto);
    Task<PlayerDto> CreateAsync(CreatePlayerDto createDto, Guid userId);
    Task<PlayerDto> UpdateAsync(Guid id, UpdatePlayerDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<bool> IsPlayerOwnedByUserAsync(Guid playerId, Guid userId);
    Task<string> UploadPlayerPhotoAsync(Guid playerId, IFormFile file, Guid userId);
    Task<IEnumerable<PlayerDto>> GetTopScorersAsync(Guid tournamentId, int limit = 10);
    Task<IEnumerable<PlayerDto>> GetTopAssistsAsync(Guid tournamentId, int limit = 10);
}
