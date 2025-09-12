using PhantomGG.Service.Interfaces;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Repository.Interfaces;

using PhantomGG.API.Mappings;
using PhantomGG.Service.Exceptions;
using PhantomGG.Models.Entities;

namespace PhantomGG.Service.Implementations;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly ICurrentUserService _currentUserService;

    public MatchService(
        IMatchRepository matchRepository,
        ITournamentRepository tournamentRepository,
        ITeamRepository teamRepository,
        ICurrentUserService currentUserService)
    {
        _matchRepository = matchRepository;
        _tournamentRepository = tournamentRepository;
        _teamRepository = teamRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<MatchDto>> GetAllAsync()
    {
        var matches = await _matchRepository.GetAllAsync();
        return matches.Select(m => m.ToMatchDto());
    }

    public async Task<MatchDto> GetByIdAsync(Guid id)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null)
            throw new ArgumentException("Match not found");

        return match.ToMatchDto();
    }

    public async Task<IEnumerable<MatchDto>> GetByTournamentAsync(Guid tournamentId)
    {
        var matches = await _matchRepository.GetByTournamentAsync(tournamentId);
        return matches.Select(m => m.ToMatchDto());
    }

    public async Task<IEnumerable<MatchDto>> GetByTeamAsync(Guid teamId)
    {
        var matches = await _matchRepository.GetByTeamAsync(teamId);
        return matches.Select(m => m.ToMatchDto());
    }

    public async Task<IEnumerable<MatchDto>> GetUpcomingMatchesAsync(Guid tournamentId)
    {
        var matches = await _matchRepository.GetUpcomingMatchesAsync(tournamentId);
        return matches.Select(m => m.ToMatchDto());
    }

    public async Task<IEnumerable<MatchDto>> GetCompletedMatchesAsync(Guid tournamentId)
    {
        var matches = await _matchRepository.GetCompletedMatchesAsync(tournamentId);
        return matches.Select(m => m.ToMatchDto());
    }

    public async Task<IEnumerable<MatchDto>> SearchAsync(MatchSearchDto searchDto)
    {
        var matches = await _matchRepository.SearchAsync(searchDto);
        return matches.Select(m => m.ToMatchDto());
    }

    public async Task<MatchDto> CreateAsync(CreateMatchDto createDto, Guid userId)
    {
        // Check if user has permission to create matches
        var tournament = await _tournamentRepository.GetByIdAsync(createDto.TournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (!await _tournamentRepository.IsOrganizerAsync(createDto.TournamentId, userId))
            throw new UnauthorizedException("You don't have permission to create matches for this tournament");

        // Validate teams exist and are part of the tournament
        var homeTeam = await _teamRepository.GetByIdAsync(createDto.HomeTeamId);
        var awayTeam = await _teamRepository.GetByIdAsync(createDto.AwayTeamId);

        if (homeTeam == null || awayTeam == null)
            throw new ArgumentException("One or both teams not found");

        if (homeTeam.TournamentId != createDto.TournamentId || awayTeam.TournamentId != createDto.TournamentId)
            throw new ArgumentException("Teams must be part of the tournament");

        // Check for scheduling conflicts
        if (await _matchRepository.TeamsHaveMatchOnDateAsync(createDto.HomeTeamId, createDto.AwayTeamId, createDto.MatchDate))
            throw new InvalidOperationException("Teams already have a match scheduled on this date");

        var match = createDto.ToMatch();
        var createdMatch = await _matchRepository.CreateAsync(match);
        return createdMatch.ToMatchDto();
    }

    public async Task<MatchDto> UpdateAsync(Guid id, UpdateMatchDto updateDto, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null)
            throw new ArgumentException("Match not found");

        if (!await _tournamentRepository.IsOrganizerAsync(id, userId))
            throw new UnauthorizedException("You don't have permission to update this match");

        match.UpdateFromDto(updateDto);
        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToMatchDto();
    }

    public async Task<MatchDto> UpdateResultAsync(Guid id, MatchResultDto resultDto, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null)
            throw new ArgumentException("Match not found");

        if (!await _tournamentRepository.IsOrganizerAsync(id, userId))
            throw new UnauthorizedException("You don't have permission to update this match result");

        // Update match result
        match.HomeScore = resultDto.HomeScore;
        match.AwayScore = resultDto.AwayScore;
        match.Status = "Completed";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToMatchDto();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null)
            throw new ArgumentException("Match not found");

        if (!await _tournamentRepository.IsOrganizerAsync(id, userId))
            throw new UnauthorizedException("You don't have permission to delete this match");

        await _matchRepository.DeleteAsync(id);
    }

    public async Task<bool> IsMatchOwnedByUserAsync(Guid matchId, Guid userId)
    {
        return await _tournamentRepository.IsOrganizerAsync(matchId, userId);
    }

    // Fixture generation - simplified for MVP
    public async Task<IEnumerable<MatchDto>> GenerateRoundRobinFixturesAsync(GenerateFixturesDto generateDto, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(generateDto.TournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (!await _tournamentRepository.IsOrganizerAsync(generateDto.TournamentId, userId))
            throw new UnauthorizedException("You don't have permission to generate fixtures for this tournament");

        var teams = await _teamRepository.GetByTournamentAsync(generateDto.TournamentId);
        var teamsList = teams.Where(t => t.RegistrationStatus == "Approved").ToList();

        if (teamsList.Count < tournament.MinTeams)
            throw new InvalidOperationException($"At least {tournament.MinTeams} approved teams are required to generate fixtures");

        var matches = new List<Match>();
        var matchDate = generateDto.StartDate;

        // Simple round-robin generation
        for (int i = 0; i < teamsList.Count; i++)
        {
            for (int j = i + 1; j < teamsList.Count; j++)
            {
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    TournamentId = generateDto.TournamentId,
                    HomeTeamId = teamsList[i].Id,
                    AwayTeamId = teamsList[j].Id,
                    MatchDate = matchDate,
                    Status = "Scheduled",
                    Venue = generateDto.DefaultVenue
                };

                matches.Add(match);
                matchDate = matchDate.AddDays(generateDto.DaysBetweenMatches);
            }
        }

        // If return matches are requested, generate them
        if (generateDto.IncludeReturnMatches)
        {
            for (int i = 0; i < teamsList.Count; i++)
            {
                for (int j = i + 1; j < teamsList.Count; j++)
                {
                    var returnMatch = new Match
                    {
                        Id = Guid.NewGuid(),
                        TournamentId = generateDto.TournamentId,
                        HomeTeamId = teamsList[j].Id, // Swapped home and away
                        AwayTeamId = teamsList[i].Id,
                        MatchDate = matchDate,
                        Status = "Scheduled",
                        Venue = generateDto.DefaultVenue
                    };

                    matches.Add(returnMatch);
                    matchDate = matchDate.AddDays(generateDto.DaysBetweenMatches);
                }
            }
        }

        // Save all matches
        var createdMatches = new List<Match>();
        foreach (var match in matches)
        {
            var createdMatch = await _matchRepository.CreateAsync(match);
            createdMatches.Add(createdMatch);
        }

        return createdMatches.Select(m => m.ToMatchDto());
    }

    public async Task<IEnumerable<MatchDto>> GenerateKnockoutFixturesAsync(GenerateFixturesDto generateDto, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(generateDto.TournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (!await _tournamentRepository.IsOrganizerAsync(generateDto.TournamentId, userId))
            throw new UnauthorizedException("You don't have permission to generate fixtures for this tournament");

        var teams = await _teamRepository.GetByTournamentAsync(generateDto.TournamentId);
        var teamsList = teams.Where(t => t.RegistrationStatus == "Approved").ToList();

        if (teamsList.Count < tournament.MinTeams)
            throw new InvalidOperationException($"At least {tournament.MinTeams} approved teams are required to generate fixtures");

        // For single elimination, we need a power of 2 number of teams
        var powerOfTwo = GetNextPowerOfTwo(teamsList.Count);
        var byesNeeded = powerOfTwo - teamsList.Count;

        var matches = new List<Match>();
        var matchDate = generateDto.StartDate;

        // Shuffle teams for fair bracket seeding
        var shuffledTeams = teamsList.OrderBy(x => Guid.NewGuid()).ToList();

        // Add bye placeholders if needed
        var bracketTeams = new List<Team?>(shuffledTeams.Cast<Team?>());
        for (int i = 0; i < byesNeeded; i++)
        {
            bracketTeams.Add(null); // null represents a bye
        }

        var currentRoundTeams = bracketTeams;
        var roundName = GetRoundName(powerOfTwo / 2);

        // Generate first round matches (with byes)
        for (int i = 0; i < currentRoundTeams.Count; i += 2)
        {
            var homeTeam = currentRoundTeams[i];
            var awayTeam = currentRoundTeams[i + 1];

            // Skip matches where both teams are byes
            if (homeTeam == null && awayTeam == null)
                continue;

            // If one team has a bye, they automatically advance
            if (homeTeam == null || awayTeam == null)
                continue;

            var match = new Match
            {
                Id = Guid.NewGuid(),
                TournamentId = generateDto.TournamentId,
                HomeTeamId = homeTeam.Id,
                AwayTeamId = awayTeam.Id,
                MatchDate = matchDate,
                Status = "Scheduled",
                Venue = generateDto.DefaultVenue
            };

            matches.Add(match);
        }

        // Generate subsequent rounds (placeholders for future matches)
        var remainingTeams = powerOfTwo / 2;
        while (remainingTeams > 1)
        {
            matchDate = matchDate.AddDays(generateDto.DaysBetweenMatches);
            remainingTeams /= 2;
            roundName = GetRoundName(remainingTeams);

            for (int i = 0; i < remainingTeams; i++)
            {
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    TournamentId = generateDto.TournamentId,
                    HomeTeamId = Guid.Empty, // TBD based on previous round results
                    AwayTeamId = Guid.Empty, // TBD based on previous round results
                    MatchDate = matchDate,
                    Status = "Pending", // Will be scheduled once teams are determined
                    Venue = generateDto.DefaultVenue
                };

                matches.Add(match);
            }
        }

        // Save all matches
        var createdMatches = new List<Match>();
        foreach (var match in matches)
        {
            var createdMatch = await _matchRepository.CreateAsync(match);
            createdMatches.Add(createdMatch);
        }

        return createdMatches.Select(m => m.ToMatchDto());
    }

    // Match status management
    public async Task<MatchDto> StartMatchAsync(Guid matchId, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new ArgumentException("Match not found");

        if (!await _tournamentRepository.IsOrganizerAsync(matchId, userId))
            throw new UnauthorizedException("You don't have permission to start this match");

        match.Status = "In Progress";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToMatchDto();
    }

    public async Task<MatchDto> EndMatchAsync(Guid matchId, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new ArgumentException("Match not found");

        if (!await _tournamentRepository.IsOrganizerAsync(matchId, userId))
            throw new UnauthorizedException("You don't have permission to end this match");

        match.Status = "Completed";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToMatchDto();
    }

    public async Task<MatchDto> CancelMatchAsync(Guid matchId, string reason, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new ArgumentException("Match not found");

        if (!await _tournamentRepository.IsOrganizerAsync(matchId, userId))
            throw new UnauthorizedException("You don't have permission to cancel this match");

        match.Status = "Cancelled";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToMatchDto();
    }

    public async Task<MatchDto> PostponeMatchAsync(Guid matchId, DateTime newDate, string reason, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new ArgumentException("Match not found");

        if (!await _tournamentRepository.IsOrganizerAsync(matchId, userId))
            throw new UnauthorizedException("You don't have permission to postpone this match");

        match.MatchDate = newDate;
        match.Status = "Postponed";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToMatchDto();
    }

    // Match events - simplified for MVP
    public Task AddMatchEventAsync(Guid matchId, CreateMatchEventDto eventDto, Guid userId)
    {
        // For MVP, just throw not implemented
        throw new NotImplementedException("Match events will be implemented in a future phase");
    }

    public Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(Guid matchId)
    {
        // For MVP, just return empty list
        return Task.FromResult<IEnumerable<MatchEventDto>>(new List<MatchEventDto>());
    }

    public async Task<IEnumerable<MatchDto>> AutoGenerateFixturesAsync(AutoGenerateFixturesDto generateDto, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(generateDto.TournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (!await _tournamentRepository.IsOrganizerAsync(generateDto.TournamentId, userId))
            throw new UnauthorizedException("You don't have permission to generate fixtures for this tournament");

        // Check if we have enough teams
        var approvedTeamCount = await _tournamentRepository.GetApprovedTeamCountAsync(generateDto.TournamentId);
        if (approvedTeamCount < tournament.MinTeams)
            throw new InvalidOperationException($"Tournament needs at least {tournament.MinTeams} approved teams to generate fixtures. Currently has {approvedTeamCount} teams.");

        // Create a GenerateFixturesDto from the AutoGenerateFixturesDto
        var fixtureDto = new GenerateFixturesDto
        {
            TournamentId = generateDto.TournamentId,
            StartDate = generateDto.StartDate,
            DaysBetweenMatches = generateDto.DaysBetweenRounds,
            DefaultVenue = generateDto.DefaultVenue,
            IncludeReturnMatches = generateDto.IncludeReturnMatches
        };

        // Generate fixtures based on tournament format
        return generateDto.TournamentFormat.ToLower() switch
        {
            "roundrobin" => await GenerateRoundRobinFixturesAsync(fixtureDto, userId),
            "singleelimination" => await GenerateKnockoutFixturesAsync(fixtureDto, userId),
            _ => throw new ArgumentException($"Unsupported tournament format: {generateDto.TournamentFormat}")
        };
    }

    public async Task<FixtureGenerationStatusDto> GetFixtureGenerationStatusAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        var approvedTeamCount = await _tournamentRepository.GetApprovedTeamCountAsync(tournamentId);

        var canGenerate = approvedTeamCount >= tournament.MinTeams &&
                         approvedTeamCount <= tournament.MaxTeams;

        string message = "";
        if (approvedTeamCount < tournament.MinTeams)
            message = $"Need at least {tournament.MinTeams} approved teams. Currently have {approvedTeamCount}.";
        else if (approvedTeamCount > tournament.MaxTeams)
            message = $"Too many teams ({approvedTeamCount}). Maximum allowed is {tournament.MaxTeams}.";
        else
            message = "Ready to generate fixtures.";

        return new FixtureGenerationStatusDto
        {
            TournamentId = tournamentId,
            TournamentName = tournament.Name,
            RegisteredTeams = approvedTeamCount,
            RequiredTeams = tournament.MinTeams,
            MaxTeams = tournament.MaxTeams,
            Status = tournament.Status,
            CanGenerateFixtures = canGenerate,
            Message = message
        };
    }

    // Helper methods for knockout tournament generation
    private static int GetNextPowerOfTwo(int number)
    {
        if (number <= 1) return 2;

        int power = 1;
        while (power < number)
        {
            power *= 2;
        }
        return power;
    }

    private static string GetRoundName(int teamsRemaining)
    {
        return teamsRemaining switch
        {
            1 => "Final",
            2 => "Semi-Final",
            4 => "Quarter-Final",
            8 => "Round of 16",
            16 => "Round of 32",
            32 => "Round of 64",
            _ => $"Round of {teamsRemaining * 2}"
        };
    }
}
