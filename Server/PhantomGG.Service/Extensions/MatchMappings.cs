using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.Entities;

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
            Venue = "",
            Status = match.Status,
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
            Status = "Scheduled",
            HomeScore = null,
            AwayScore = null
        };
    }

    public static void UpdateEntity(this UpdateMatchDto updateDto, Match match)
    {
        match.MatchDate = updateDto.MatchDate;
        if (!string.IsNullOrEmpty(updateDto.Status))
            match.Status = updateDto.Status;
    }
}