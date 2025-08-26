using PhantomGG.API.DTOs.Team;
using PhantomGG.API.Models;

namespace PhantomGG.API.Mappings;

public static class TeamMappings
{
    public static TeamDto ToTeamDto(this Team team)
    {
        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Manager = team.Manager,
            NumberOfPlayers = team.NumberOfPlayers,
            LogoUrl = team.LogoUrl,
            TournamentId = team.TournamentId,
            TournamentName = team.Tournament?.Name ?? "Unknown",
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt
        };
    }

    public static Team ToTeam(this CreateTeamDto dto)
    {
        return new Team
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Manager = dto.Manager,
            NumberOfPlayers = dto.NumberOfPlayers,
            LogoUrl = dto.LogoUrl,
            TournamentId = dto.TournamentId,
            IsActive = true
        };
    }

    public static void UpdateFromDto(this Team team, UpdateTeamDto dto)
    {
        team.Name = dto.Name;
        team.Manager = dto.Manager;
        team.NumberOfPlayers = dto.NumberOfPlayers;
        team.LogoUrl = dto.LogoUrl;
        team.UpdatedAt = DateTime.UtcNow;
    }
}
