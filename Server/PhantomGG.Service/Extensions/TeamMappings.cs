using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Repository.Entities;
using System.Text;
using System.Text.Json;

namespace PhantomGG.Service.Mappings;

public static class TeamMappings
{
    public static TeamDto ToDto(this Team team)
    {
        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            ShortName = team.ShortName,
            LogoUrl = team.LogoUrl,
            UserId = team.UserId,
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt
        };
    }

    public static Team ToEntity(this CreateTeamDto createDto, Guid userId)
    {
        return new Team
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            ShortName = createDto.ShortName ?? string.Empty,
            //LogoUrl = createDto.LogoUrl,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this UpdateTeamDto updateDto, Team team)
    {
        if (!string.IsNullOrEmpty(updateDto.Name))
            team.Name = updateDto.Name;
        if (!string.IsNullOrEmpty(updateDto.ShortName))
            team.ShortName = updateDto.ShortName;
        //if (updateDto.LogoUrl != null)
        //    team.LogoUrl = updateDto.LogoUrl;

        team.UpdatedAt = DateTime.UtcNow;
    }

    public static TournamentTeamDto ToDto(this TournamentTeam tournamentTeam)
    {
        return new TournamentTeamDto
        {
            Id = tournamentTeam.TeamId,
            Name = tournamentTeam.Team.Name,
            ShortName = tournamentTeam.Team.ShortName,
            LogoUrl = tournamentTeam.Team.LogoUrl,
            Status = tournamentTeam.Status,
            RegisteredAt = tournamentTeam.RequestedAt,
            AcceptedAt = tournamentTeam.AcceptedAt,
            ManagerName = $"{tournamentTeam.Team.User.FirstName} {tournamentTeam.Team.User.LastName}".Trim(),
            ManagerId = tournamentTeam.Team.UserId
        };
    }
    public static string GetDeterministicKey(this TeamSearchDto dto)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto))
        );
    }
}