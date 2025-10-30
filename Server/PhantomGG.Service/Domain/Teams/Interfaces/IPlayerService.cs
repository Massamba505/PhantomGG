using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Service.Domain.Teams.Interfaces;

public interface IPlayerService
{
    Task<PlayerDto> GetByIdAsync(Guid playerId);
    Task<PlayerDto> CreateAsync(CreatePlayerDto createDto);
    Task<PlayerDto> UpdateAsync(UpdatePlayerDto updateDto, Guid playerId);
    Task DeleteAsync(Guid teamId, Guid playerId);
    Task<IEnumerable<PlayerDto>> GetByTeamAsync(Guid teamId);
}
