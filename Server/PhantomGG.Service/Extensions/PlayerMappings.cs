using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.Entities;

namespace PhantomGG.Service.Mappings;

public static class PlayerMappings
{
    public static PlayerDto ToDto(this Player player)
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
            UpdatedAt = null,
            IsActive = true
        };
    }

    public static Player ToEntity(this CreatePlayerDto createDto)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Position = createDto.Position,
            Email = createDto.Email,
            PhotoUrl = createDto.PhotoUrl,
            TeamId = createDto.TeamId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this UpdatePlayerDto updateDto, Player player)
    {
        if (!string.IsNullOrEmpty(updateDto.FirstName))
            player.FirstName = updateDto.FirstName;
        if (!string.IsNullOrEmpty(updateDto.LastName))
            player.LastName = updateDto.LastName;
        if (updateDto.Position != null)
            player.Position = updateDto.Position;
        if (updateDto.Email != null)
            player.Email = updateDto.Email;
        if (updateDto.PhotoUrl != null)
            player.PhotoUrl = updateDto.PhotoUrl;
    }
}