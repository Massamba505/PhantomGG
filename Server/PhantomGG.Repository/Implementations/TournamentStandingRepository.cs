using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Models.DTOs.TournamentStanding;
using PhantomGG.Common.Enums;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Implementations;

public class TournamentStandingRepository(PhantomContext context) : ITournamentStandingRepository
{
    private readonly PhantomContext _context = context;

    public async Task<IEnumerable<TournamentStandingDto>> GetByTournamentAsync(Guid tournamentId)
    {
        var completedMatches = await _context.Matches
            .Where(m => m.TournamentId == tournamentId && (MatchStatus)m.Status == MatchStatus.Completed)
            .ToListAsync();

        var teams = await _context.Teams
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.LogoUrl,
            })
            .ToListAsync();

        var standings = new List<TournamentStandingDto>();

        foreach (var team in teams)
        {
            var teamMatches = completedMatches
                .Where(m => m.HomeTeamId == team.Id || m.AwayTeamId == team.Id)
                .ToList();

            var wins = teamMatches.Count(m =>
                (m.HomeTeamId == team.Id && m.HomeScore > m.AwayScore) ||
                (m.AwayTeamId == team.Id && m.AwayScore > m.HomeScore));

            var draws = teamMatches.Count(m => m.HomeScore == m.AwayScore);

            var losses = teamMatches.Count(m =>
                (m.HomeTeamId == team.Id && m.HomeScore < m.AwayScore) ||
                (m.AwayTeamId == team.Id && m.AwayScore < m.HomeScore));

            var goalsFor = teamMatches.Sum(m =>
                (m.HomeTeamId == team.Id ? m.HomeScore ?? 0 : 0) +
                (m.AwayTeamId == team.Id ? m.AwayScore ?? 0 : 0));

            var goalsAgainst = teamMatches.Sum(m =>
                (m.HomeTeamId == team.Id ? m.AwayScore ?? 0 : 0) +
                (m.AwayTeamId == team.Id ? m.HomeScore ?? 0 : 0));

            var standing = new TournamentStandingDto
            {
                TeamId = team.Id,
                TeamName = team.Name,
                TeamLogo = team.LogoUrl,
                MatchesPlayed = teamMatches.Count,
                Wins = wins,
                Draws = draws,
                Losses = losses,
                GoalsFor = goalsFor,
                GoalsAgainst = goalsAgainst,
                Points = (wins * 3) + draws
            };

            standings.Add(standing);
        }

        var orderedStandings = standings
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .ToList();

        for (int i = 0; i < orderedStandings.Count; i++)
        {
            orderedStandings[i].Position = i + 1;
        }

        return orderedStandings;
    }


    public async Task<IEnumerable<PlayerGoalStandingDto>> GetPlayerGoalStandingsAsync(Guid tournamentId)
    {
        var goalEvents = await _context.MatchEvents
            .Include(me => me.Match)
            .Include(me => me.Player)
            .Include(me => me.Team)
            .Where(me => me.Match.TournamentId == tournamentId &&
                         (MatchEventType)me.EventType == MatchEventType.Goal)
            .ToListAsync();

        var completedMatches = await _context.Matches
            .Where(m => m.TournamentId == tournamentId && (MatchStatus)m.Status == MatchStatus.Completed)
            .ToListAsync();
        var playerGoalGroups = goalEvents
            .GroupBy(ge => ge.PlayerId)
            .Select(g =>
            {
                var firstEvent = g.First();
                var player = firstEvent.Player;
                var team = firstEvent.Team;

                var teamMatches = completedMatches
                    .Where(m => m.HomeTeamId == team.Id || m.AwayTeamId == team.Id)
                    .Count();

                return new PlayerGoalStandingDto
                {
                    PlayerId = g.Key,
                    PlayerName = player != null ? $"{player.FirstName} {player.LastName}" : "Unknown Player",
                    PlayerPhoto = player?.PhotoUrl,
                    TeamId = team.Id,
                    TeamName = team.Name,
                    TeamLogo = team.LogoUrl,
                    Goals = g.Count(),
                    MatchesPlayed = teamMatches
                };
            })
            .OrderByDescending(p => p.Goals)
            .ThenBy(p => p.PlayerName)
            .ToList();

        for (int i = 0; i < playerGoalGroups.Count; i++)
        {
            playerGoalGroups[i].Position = i + 1;
        }

        return playerGoalGroups;
    }


    public async Task<IEnumerable<PlayerAssistStandingDto>> GetPlayerAssistStandingsAsync(Guid tournamentId)
    {
        var assistEvents = await _context.MatchEvents
            .Include(me => me.Match)
            .Include(me => me.Player)
            .Include(me => me.Team)
            .Where(me => me.Match.TournamentId == tournamentId &&
                         (MatchEventType)me.EventType == MatchEventType.Assist)
            .ToListAsync();

        var completedMatches = await _context.Matches
            .Where(m => m.TournamentId == tournamentId && (MatchStatus)m.Status == MatchStatus.Completed)
            .ToListAsync();

        var playerAssistGroups = assistEvents
            .GroupBy(ae => ae.PlayerId)
            .Select(g =>
            {
                var firstEvent = g.First();
                var player = firstEvent.Player;
                var team = firstEvent.Team;

                var teamMatches = completedMatches
                    .Where(m => m.HomeTeamId == team.Id || m.AwayTeamId == team.Id)
                    .Count();

                return new PlayerAssistStandingDto
                {
                    PlayerId = g.Key,
                    PlayerName = player != null ? $"{player.FirstName} {player.LastName}" : "Unknown Player",
                    PlayerPhoto = player?.PhotoUrl,
                    TeamId = team.Id,
                    TeamName = team.Name,
                    TeamLogo = team.LogoUrl,
                    Assists = g.Count(),
                    MatchesPlayed = teamMatches
                };
            })
            .OrderByDescending(p => p.Assists)
            .ThenByDescending(p => p.AssistsPerMatch)
            .ToList();

        for (int i = 0; i < playerAssistGroups.Count; i++)
        {
            playerAssistGroups[i].Position = i + 1;
        }

        return playerAssistGroups;
    }
}
