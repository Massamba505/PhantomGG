using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Specifications;
using System.Text;
using System.Text.Json;

namespace PhantomGG.Service.Mappings;

public static class TeamMappings
{
    public static TeamDto ToDto(this Team team)
    {
        var players = team.Players.Select(p => p.ToDto());

        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            ShortName = team.ShortName,
            LogoUrl = team.LogoUrl,
            UserId = team.UserId,
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt,
            CountPlayers = team.Players.Count,
            players = players
        };
    }

    public static Team ToEntity(this CreateTeamDto createDto, Guid userId)
    {
        return new Team
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            ShortName = createDto.ShortName ?? string.Empty,
            UserId = userId,
            LogoUrl = $"https://placehold.co/200x200?text={createDto.Name}",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this UpdateTeamDto updateDto, Team team)
    {
        if (!string.IsNullOrEmpty(updateDto.Name))
            team.Name = updateDto.Name;
        if (!string.IsNullOrEmpty(updateDto.ShortName))
            team.ShortName = updateDto.ShortName;

        team.UpdatedAt = DateTime.UtcNow;
    }

    public static TournamentTeamDto ToDto(this TournamentTeam tournamentTeam)
    {
        var players = tournamentTeam.Team.Players.Select(p => p.ToDto());
        return new TournamentTeamDto
        {
            Id = tournamentTeam.TeamId,
            TeamId = tournamentTeam.TeamId,
            TournamentId = tournamentTeam.TournamentId,
            Name = tournamentTeam.Team.Name,
            ShortName = tournamentTeam.Team.ShortName,
            LogoUrl = tournamentTeam.Team.LogoUrl,
            TournamentName = tournamentTeam.Tournament?.Name,
            Status = (TeamRegistrationStatus)tournamentTeam.Status,
            RegisteredAt = tournamentTeam.RequestedAt,
            RequestedAt = tournamentTeam.RequestedAt,
            AcceptedAt = tournamentTeam.AcceptedAt,
            ManagerName = $"{tournamentTeam.Team.User.FirstName} {tournamentTeam.Team.User.LastName}".Trim(),
            ManagerId = tournamentTeam.Team.UserId,
            CountPlayers = players.Count(),
            Players = players
        };
    }

    public static string GetDeterministicKey(this TeamSpecification dto)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto))
        );
    }
}