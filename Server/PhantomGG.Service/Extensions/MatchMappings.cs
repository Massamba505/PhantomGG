using PhantomGG.Models.Entities;
using PhantomGG.Models.DTOs.Match;

namespace PhantomGG.API.Mappings;

public static class MatchMappings
{
    public static MatchDto ToMatchDto(this Match match)
    {
        return new MatchDto
        {
            Id = match.Id,
            TournamentId = match.TournamentId,
            TournamentName = match.Tournament?.Name ?? string.Empty,
            HomeTeamId = match.HomeTeamId,
            HomeTeamName = match.HomeTeam?.Name ?? "TBD",
            HomeTeamLogo = match.HomeTeam?.LogoUrl,
            AwayTeamId = match.AwayTeamId,
            AwayTeamName = match.AwayTeam?.Name ?? "TBD",
            AwayTeamLogo = match.AwayTeam?.LogoUrl,
            MatchDate = match.MatchDate,
            // Venue = match.Venue,
            Status = match.Status,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore
        };
    }

    public static Match ToMatch(this CreateMatchDto createDto)
    {
        return new Match
        {
            Id = Guid.NewGuid(),
            TournamentId = createDto.TournamentId,
            HomeTeamId = createDto.HomeTeamId,
            AwayTeamId = createDto.AwayTeamId,
            MatchDate = createDto.MatchDate,
            // Venue = createDto.Venue,
            Status = "Scheduled"
        };
    }

    public static void UpdateFromDto(this Match match, UpdateMatchDto updateDto)
    {
        match.MatchDate = updateDto.MatchDate;
        // match.Venue = updateDto.Venue;
        match.Status = updateDto.Status;
    }

    public static void UpdateResult(this Match match, MatchResultDto resultDto)
    {
        match.HomeScore = resultDto.HomeScore;
        match.AwayScore = resultDto.AwayScore;
        match.Status = "Completed";
    }
}
