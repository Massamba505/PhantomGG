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
            ShortName = team.ShortName,
            ManagerName = team.ManagerName,
            ManagerEmail = team.ManagerEmail,
            ManagerPhone = team.ManagerPhone,
            LogoUrl = team.LogoUrl,
            TeamPhotoUrl = team.TeamPhotoUrl,
            TournamentId = team.TournamentId,
            TournamentName = team.Tournament?.Name ?? "Unknown",
            RegistrationStatus = team.RegistrationStatus,
            RegistrationDate = team.RegistrationDate,
            ApprovedDate = team.ApprovedDate,
            NumberOfPlayers = team.NumberOfPlayers,
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt,
            IsActive = team.IsActive
        };
    }

    public static Team ToTeam(this CreateTeamDto dto)
    {
        return new Team
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            ShortName = dto.ShortName,
            ManagerName = dto.ManagerName,
            ManagerEmail = dto.ManagerEmail,
            ManagerPhone = dto.ManagerPhone,
            LogoUrl = dto.LogoUrl,
            TeamPhotoUrl = dto.TeamPhotoUrl,
            TournamentId = dto.TournamentId,
            RegistrationStatus = "Pending",
            NumberOfPlayers = 0,
            IsActive = true
        };
    }

    public static void UpdateFromDto(this Team team, UpdateTeamDto dto)
    {
        team.Name = dto.Name;
        team.ShortName = dto.ShortName;
        team.ManagerName = dto.ManagerName;
        team.ManagerEmail = dto.ManagerEmail;
        team.ManagerPhone = dto.ManagerPhone;
        team.LogoUrl = dto.LogoUrl;
        team.TeamPhotoUrl = dto.TeamPhotoUrl;
        team.UpdatedAt = DateTime.UtcNow;
    }
}
