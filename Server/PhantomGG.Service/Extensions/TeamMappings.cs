using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.Entities;

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
}