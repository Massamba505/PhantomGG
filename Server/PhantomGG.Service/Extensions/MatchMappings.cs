using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Specifications;
using System.Text;
using System.Text.Json;

namespace PhantomGG.Service.Mappings;

public static class MatchMappings
{
    public static MatchDto ToDto(this Match match)
    {
        return new MatchDto
        {
            Id = match.Id,
            TournamentId = match.TournamentId,
            TournamentName = match.Tournament?.Name ?? "Unknown",
            HomeTeamId = match.HomeTeamId,
            HomeTeamName = match.HomeTeam?.Name ?? "Unknown",
            HomeTeamLogo = match.HomeTeam?.LogoUrl,
            AwayTeamId = match.AwayTeamId,
            AwayTeamName = match.AwayTeam?.Name ?? "Unknown",
            AwayTeamLogo = match.AwayTeam?.LogoUrl,
            MatchDate = match.MatchDate,
            Venue = match.Tournament?.Location ?? "TBD",
            Status = (MatchStatus)match.Status,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore
        };
    }

    public static Match ToEntity(this CreateMatchDto createDto)
    {
        return new Match
        {
            Id = Guid.NewGuid(),
            TournamentId = createDto.TournamentId,
            HomeTeamId = createDto.HomeTeamId,
            AwayTeamId = createDto.AwayTeamId,
            MatchDate = createDto.MatchDate,
            Status = (int)MatchStatus.Scheduled,
            HomeScore = null,
            AwayScore = null
        };
    }

    public static void UpdateEntity(this UpdateMatchDto updateDto, Match match)
    {
        match.MatchDate = updateDto.MatchDate;
        match.Status = (int)updateDto.Status;
    }

    public static string GetDeterministicKey(this MatchSpecification dto)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto))
        );
    }
}