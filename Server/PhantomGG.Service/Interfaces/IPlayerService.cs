using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Service.Interfaces;

public interface IPlayerService
{
    #region Basic Player Operations (if needed separately)
    Task<PlayerDto> GetByIdAsync(Guid id);
    Task<PlayerDto> CreateAsync(CreatePlayerDto createDto, Guid teamId, Guid userId);
    Task<PlayerDto> UpdateAsync(Guid id, UpdatePlayerDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    #endregion
}
