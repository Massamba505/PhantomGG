using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.Entities;

namespace PhantomGG.API.Mappings;

public static class PlayerMappings
{
    public static PlayerDto ToPlayerDto(this Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            FullName = $"{player.FirstName} {player.LastName}",
            Position = player.Position,
            Email = player.Email,
            PhotoUrl = player.PhotoUrl,
            TeamId = player.TeamId,
            TeamName = player.Team?.Name ?? "Unknown",
            CreatedAt = player.CreatedAt,
            // UpdatedAt = player.UpdatedAt
        };
    }

    public static Player ToPlayer(this CreatePlayerDto dto)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Position = dto.Position,
            Email = dto.Email,
            PhotoUrl = dto.PhotoUrl,
            TeamId = dto.TeamId
        };
    }

    public static void UpdateFromDto(this Player player, UpdatePlayerDto dto)
    {
        player.FirstName = dto.FirstName;
        player.LastName = dto.LastName;
        player.Position = dto.Position;
        player.Email = dto.Email;
        player.PhotoUrl = dto.PhotoUrl;
        // player.UpdatedAt = DateTime.UtcNow;
    }
}
