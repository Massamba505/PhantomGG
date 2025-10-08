using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations;

public class TournamentMatchService(
    ITournamentMatchRepository tournamentMatchRepository,
    ITournamentTeamRepository tournamentTeamRepository,
    IMatchRepository matchRepository,
    ITournamentValidationService validationService) : ITournamentMatchService
{
    private readonly ITournamentMatchRepository _tournamentMatchRepository = tournamentMatchRepository;
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;
    private readonly IMatchRepository _matchRepository = matchRepository;
    private readonly ITournamentValidationService _validationService = validationService;

    public async Task<IEnumerable<MatchDto>> GetMatchesAsync(Guid tournamentId, MatchStatus? status)
    {
        if (status.HasValue)
        {
            var matchesWithStatus = await _tournamentMatchRepository.GetByTournamentAndStatusAsync(tournamentId, status.Value.ToString());
            return matchesWithStatus.Select(tt => tt.ToDto());
        }

        var matches = await _tournamentMatchRepository.GetByTournamentAsync(tournamentId);
        return matches.Select(m => m.ToDto());
    }

    public async Task CreateBracketAsync(Guid tournamentId, Guid organizerId)
    {
        await _validationService.ValidateCanManageTeamsAsync(tournamentId, organizerId);

        var approvedTournamentTeams = await _tournamentTeamRepository.GetByTournamentAndStatusAsync(tournamentId, TeamRegistrationStatus.Approved.ToString());
        var teams = approvedTournamentTeams.Select(tt => tt.Team).ToList();

        if (teams.Count < 2)
        {
            throw new InvalidOperationException("Tournament must have at least 2 approved teams to generate fixtures");
        }

        var existingMatches = await _tournamentMatchRepository.GetByTournamentAsync(tournamentId);
        if (existingMatches.Any())
        {
            throw new InvalidOperationException("Tournament already has fixtures generated");
        }

        var fixtures = GenerateRoundRobinFixtures(teams, tournamentId);

        foreach (var fixture in fixtures)
        {
            await _matchRepository.CreateAsync(fixture);
        }
    }

    private static List<Match> GenerateRoundRobinFixtures(List<Team> teams, Guid tournamentId)
    {
        var fixtures = new List<Match>();
        var teamCount = teams.Count;
        var rounds = teamCount % 2 == 0 ? teamCount - 1 : teamCount;
        var matchesPerRound = teamCount / 2;

        var teamIndices = Enumerable.Range(0, teamCount).ToList();
        var baseDate = DateTime.UtcNow.AddDays(7);

        for (int round = 0; round < rounds; round++)
        {
            var roundDate = baseDate.AddDays(round * 7);

            for (int match = 0; match < matchesPerRound; match++)
            {
                var homeIndex = teamIndices[match];
                var awayIndex = teamIndices[teamCount - 1 - match];

                if (homeIndex >= teamCount || awayIndex >= teamCount)
                    continue;

                var homeTeam = teams[homeIndex];
                var awayTeam = teams[awayIndex];

                var fixture = new Match
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    HomeTeamId = homeTeam.Id,
                    AwayTeamId = awayTeam.Id,
                    MatchDate = roundDate.AddHours(15),
                    Status = MatchStatus.Scheduled.ToString(),
                    HomeScore = null,
                    AwayScore = null
                };

                fixtures.Add(fixture);
            }

            if (teamCount > 2)
            {
                var temp = teamIndices[1];
                for (int i = 1; i < teamCount - 1; i++)
                {
                    teamIndices[i] = teamIndices[i + 1];
                }
                teamIndices[teamCount - 1] = temp;
            }
        }

        return fixtures;
    }
}
