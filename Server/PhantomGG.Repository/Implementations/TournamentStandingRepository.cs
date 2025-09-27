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
            .Where(m => m.TournamentId == tournamentId && m.Status == MatchStatus.Completed.ToString())
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
        var completedMatches = await _context.Matches
            .Where(m => m.TournamentId == tournamentId && m.Status == MatchStatus.Completed.ToString())
            .ToListAsync();

        var matchesPlayedByTeam = completedMatches
            .SelectMany(m => new[] { m.HomeTeamId, m.AwayTeamId })
            .GroupBy(teamId => teamId)
            .ToDictionary(g => g.Key, g => g.Count());

        var goalGroups = await _context.MatchEvents
            .Where(me => me.Match.TournamentId == tournamentId &&
                         me.EventType == "Goal")
            .GroupBy(me => new { me.TeamId })
            .Select(g => new
            {
                PlayerId = Guid.Empty,
                TeamId = g.Key.TeamId,
                Goals = g.Count()
            })
            .ToListAsync();

        var teamLookup = await _context.Teams
            .Where(t => goalGroups.Select(g => g.TeamId).Distinct().Contains(t.Id))
            .ToDictionaryAsync(t => t.Id);

        var playerLookup = await _context.Players
            .Where(p => goalGroups.Select(g => g.PlayerId).Distinct().Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var standings = goalGroups.Select(g =>
        {
            var team = teamLookup[g.TeamId];
            var player = playerLookup.ContainsKey(g.PlayerId) ? playerLookup[g.PlayerId] : null;

            return new PlayerGoalStandingDto
            {
                PlayerId = g.PlayerId,
                PlayerName = string.Empty,
                PlayerPhoto = player?.PhotoUrl,
                TeamId = g.TeamId,
                TeamName = team.Name,
                TeamLogo = team.LogoUrl,
                Goals = g.Goals,
                MatchesPlayed = matchesPlayedByTeam.ContainsKey(g.TeamId) ? matchesPlayedByTeam[g.TeamId] : 0
            };
        })
        .OrderByDescending(p => p.Goals)
        .ThenBy(p => p.PlayerName)
        .ToList();

        for (int i = 0; i < standings.Count; i++)
        {
            standings[i].Position = i + 1;
        }

        return standings;
    }


    public async Task<IEnumerable<PlayerAssistStandingDto>> GetPlayerAssistStandingsAsync(Guid tournamentId)
    {
        // Get assist providers from match events
        var assistStandings = await _context.MatchEvents
            .Where(me => me.Match.TournamentId == tournamentId &&
                        me.EventType == "Assist")
            .GroupBy(me => new { me.TeamId })
            .Select(g => new
            {
                TeamId = g.Key.TeamId,
                Assists = g.Count()
            })
            .Join(_context.Teams,
                assists => assists.TeamId,
                team => team.Id,
                (assists, team) => new PlayerAssistStandingDto
                {
                    PlayerId = Guid.Empty, // We don't have player IDs in events, using player name
                    PlayerName = string.Empty,
                    TeamId = assists.TeamId,
                    TeamName = team.Name,
                    TeamLogo = team.LogoUrl,
                    Assists = assists.Assists,
                    MatchesPlayed = _context.Matches
                        .Where(m => m.TournamentId == tournamentId &&
                                   (m.HomeTeamId == assists.TeamId || m.AwayTeamId == assists.TeamId) &&
                                   m.Status == MatchStatus.Completed.ToString())
                        .Count()
                })
            .Where(p => p.Assists > 0)
            .OrderByDescending(p => p.Assists)
            .ThenByDescending(p => p.AssistsPerMatch)
            .ToListAsync();

        // Assign positions
        for (int i = 0; i < assistStandings.Count; i++)
        {
            assistStandings[i].Position = i + 1;
        }

        return assistStandings;
    }
}
