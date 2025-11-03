using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Validation.Interfaces;

public interface IPlayerValidationService
{
    Task<Player> ValidatePlayerExistsAsync(Guid playerId);
    Task<Player> ValidateCanUpdatePlayerAsync(Guid playerId, Guid userId);
    Task<Player> ValidateCanDeletePlayerAsync(Guid playerId, Guid userId);
    Task ValidateMaxPlayersPerTeamAsync(Guid teamId, int maxPlayers = 15);
    Task ValidateMinPlayersPerTeamAsync(Guid teamId, int minPlayers = 1);
    Task ValidatePlayerBelongsToTeamAsync(Guid playerId, Guid teamId);
    Task ValidatePlayerNotInMatchAsync(Guid playerId);
    Task ValidatePlayerPositionDistributionAsync(Guid teamId, int position);
    Task ValidateEmailUniquenessWithinTeamAsync(string email, Guid teamId, Guid? excludePlayerId = null);
}
