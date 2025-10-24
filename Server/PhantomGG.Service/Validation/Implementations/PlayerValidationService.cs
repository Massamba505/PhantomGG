using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Validation.Implementations;

public class PlayerValidationService(
    IPlayerRepository playerRepository,
    ITeamRepository teamRepository,
    IMatchEventRepository matchEventRepository) : IPlayerValidationService
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly IMatchEventRepository _matchEventRepository = matchEventRepository;

    public async Task<Player> ValidatePlayerExistsAsync(Guid playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new NotFoundException("Player not found");

        return player;
    }

    public async Task<Player> ValidateCanUpdatePlayerAsync(Guid playerId, Guid userId)
    {
        var player = await ValidatePlayerExistsAsync(playerId);
        var team = await _teamRepository.GetByIdAsync(player.TeamId);

        if (team == null)
            throw new NotFoundException("Team not found");

        if (team.UserId != userId)
            throw new ForbiddenException("You don't have permission to update this player");

        return player;
    }

    public async Task<Player> ValidateCanDeletePlayerAsync(Guid playerId, Guid userId)
    {
        var player = await ValidatePlayerExistsAsync(playerId);
        var team = await _teamRepository.GetByIdAsync(player.TeamId);

        if (team == null)
            throw new NotFoundException("Team not found");

        if (team.UserId != userId)
            throw new ForbiddenException("You don't have permission to delete this player");

        await ValidatePlayerNotInMatchAsync(playerId);

        var currentPlayers = await _playerRepository.GetByTeamAsync(player.TeamId);
        if (currentPlayers.Count() <= 1)
            throw new ForbiddenException("Cannot delete player. Team must have at least one player");

        return player;
    }

    public async Task ValidateMaxPlayersPerTeamAsync(Guid teamId, int maxPlayers = 15)
    {
        var existingPlayers = await _playerRepository.GetByTeamAsync(teamId);
        if (existingPlayers.Count() >= maxPlayers)
            throw new ForbiddenException($"A team cannot have more than {maxPlayers} players. Please remove a player first");
    }

    public async Task ValidateMinPlayersPerTeamAsync(Guid teamId, int minPlayers = 1)
    {
        var existingPlayers = await _playerRepository.GetByTeamAsync(teamId);
        if (existingPlayers.Count() < minPlayers)
            throw new ValidationException($"Team must have at least {minPlayers} player(s)");
    }

    public async Task ValidatePlayerBelongsToTeamAsync(Guid playerId, Guid teamId)
    {
        var player = await ValidatePlayerExistsAsync(playerId);
        if (player.TeamId != teamId)
            throw new ForbiddenException("Player does not belong to this team");
    }

    public async Task ValidatePlayerNotInMatchAsync(Guid playerId)
    {
        var matchEvents = await _matchEventRepository.GetByPlayerIdAsync(playerId);
        if (matchEvents.Any())
            throw new ForbiddenException("Cannot delete player who has participated in matches");
    }

    public async Task ValidatePlayerPositionDistributionAsync(Guid teamId, int position)
    {
        var players = await _playerRepository.GetByTeamAsync(teamId);
        var positionCounts = players.GroupBy(p => p.Position).ToDictionary(g => g.Key, g => g.Count());

        var playerPosition = (PlayerPosition)position;

        switch (playerPosition)
        {
            case PlayerPosition.Goalkeeper:
                if (positionCounts.GetValueOrDefault((int)PlayerPosition.Goalkeeper, 0) >= 3)
                    throw new ValidationException("Team can have a maximum of 3 goalkeepers");
                break;
            case PlayerPosition.Defender:
                if (positionCounts.GetValueOrDefault((int)PlayerPosition.Defender, 0) >= 6)
                    throw new ValidationException("Team can have a maximum of 6 defenders");
                break;
            case PlayerPosition.Midfielder:
                if (positionCounts.GetValueOrDefault((int)PlayerPosition.Midfielder, 0) >= 6)
                    throw new ValidationException("Team can have a maximum of 6 midfielders");
                break;
            case PlayerPosition.Forward:
                break;
        }
    }

    public async Task ValidateEmailUniquenessWithinTeamAsync(string email, Guid teamId, Guid? excludePlayerId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return;

        var players = await _playerRepository.GetByTeamAsync(teamId);
        var existingPlayer = players.FirstOrDefault(p =>
            !string.IsNullOrEmpty(p.Email) &&
            p.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            (!excludePlayerId.HasValue || p.Id != excludePlayerId.Value));

        if (existingPlayer != null)
            throw new ConflictException("A player with this email already exists in this team");
    }
}
