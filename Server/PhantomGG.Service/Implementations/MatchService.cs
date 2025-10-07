using PhantomGG.Service.Interfaces;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Repository.Entities;
using PhantomGG.Service.Mappings;
using Microsoft.Extensions.Caching.Hybrid;

namespace PhantomGG.Service.Implementations;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IMatchEventRepository _matchEventRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheInvalidationService _cacheInvalidationService;
    private readonly HybridCache _cache;

    public MatchService(
        IMatchRepository matchRepository,
        ITournamentRepository tournamentRepository,
        ITeamRepository teamRepository,
        IMatchEventRepository matchEventRepository,
        IPlayerRepository playerRepository,
        ICurrentUserService currentUserService,
        ICacheInvalidationService cacheInvalidationService,
        HybridCache cache)
    {
        _matchRepository = matchRepository;
        _tournamentRepository = tournamentRepository;
        _teamRepository = teamRepository;
        _matchEventRepository = matchEventRepository;
        _playerRepository = playerRepository;
        _currentUserService = currentUserService;
        _cacheInvalidationService = cacheInvalidationService;
        _cache = cache;
    }

    public async Task<MatchDto> GetByIdAsync(Guid id)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null)
            throw new NotFoundException("Match not found");

        return match.ToDto();
    }

    public async Task<IEnumerable<MatchDto>> GetByTournamentAsync(Guid tournamentId)
    {
        var matches = await _matchRepository.GetByTournamentAsync(tournamentId);
        return matches.Select(m => m.ToDto());
    }

    public async Task<MatchDto> UpdateResultAsync(Guid matchId, object resultDto, Guid organizerId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");

        var tournament = await _tournamentRepository.GetByIdAsync(match.TournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        if (tournament.OrganizerId != organizerId)
            throw new UnauthorizedException("You don't have permission to update this match");

        if (resultDto is not MatchResultDto matchResult)
            throw new ValidationException("Invalid result data");

        match.HomeScore = matchResult.HomeScore;
        match.AwayScore = matchResult.AwayScore;
        match.Status = Common.Enums.MatchStatus.Completed.ToString();

        var updatedMatch = await _matchRepository.UpdateAsync(match);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(matchId);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(match.TournamentId);

        return updatedMatch.ToDto();
    }

    public async Task<IEnumerable<MatchDto>> GenerateTournamentBracketAsync(Guid tournamentId, Guid organizerId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        if (tournament.OrganizerId != organizerId)
            throw new UnauthorizedException("You don't have permission to generate brackets for this tournament");

        var existingMatches = await _matchRepository.GetByTournamentAsync(tournamentId);
        return existingMatches.Select(m => m.ToDto());
    }

    public async Task<IEnumerable<MatchDto>> GetByTeamAsync(Guid teamId)
    {
        var matches = await _matchRepository.GetByTeamAsync(teamId);
        return matches.Select(m => m.ToDto());
    }

    public async Task<IEnumerable<MatchDto>> GetUpcomingMatchesAsync(Guid tournamentId)
    {
        var matches = await _matchRepository.GetUpcomingMatchesAsync(tournamentId);
        return matches.Select(m => m.ToDto());
    }

    public async Task<IEnumerable<MatchDto>> GetCompletedMatchesAsync(Guid tournamentId)
    {
        var matches = await _matchRepository.GetCompletedMatchesAsync(tournamentId);
        return matches.Select(m => m.ToDto());
    }

    public async Task<IEnumerable<MatchDto>> SearchAsync(MatchSearchDto searchDto)
    {
        var matches = await _matchRepository.SearchAsync(searchDto);
        return matches.Select(m => m.ToDto());
    }

    public async Task<MatchDto> CreateAsync(CreateMatchDto createDto, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(createDto.TournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");


        var homeTeam = await _teamRepository.GetByIdAsync(createDto.HomeTeamId);
        var awayTeam = await _teamRepository.GetByIdAsync(createDto.AwayTeamId);

        if (homeTeam == null || awayTeam == null)
            throw new NotFoundException("One or both teams not found");


        if (await _matchRepository.TeamsHaveMatchOnDateAsync(createDto.HomeTeamId, createDto.AwayTeamId, createDto.MatchDate))
            throw new ForbiddenException("Teams already have a match scheduled on this date");

        var match = createDto.ToEntity();
        var createdMatch = await _matchRepository.CreateAsync(match);
        return createdMatch.ToDto();
    }

    public async Task<MatchDto> UpdateAsync(Guid id, UpdateMatchDto updateDto, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null)
            throw new NotFoundException("Match not found");


        updateDto.UpdateEntity(match);
        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToDto();
    }

    public async Task<MatchDto> UpdateResultAsync(Guid id, MatchResultDto resultDto, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null)
            throw new NotFoundException("Match not found");


        match.HomeScore = resultDto.HomeScore;
        match.AwayScore = resultDto.AwayScore;
        match.Status = "Completed";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToDto();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null)
            throw new NotFoundException("Match not found");


        await _matchRepository.DeleteAsync(id);
    }


    public async Task<IEnumerable<MatchDto>> GenerateRoundRobinFixturesAsync(GenerateFixturesDto generateDto, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(generateDto.TournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");


        var teams = await _tournamentRepository.GetTournamentTeamsAsync(generateDto.TournamentId);
        var teamsList = teams.ToList();

        if (teamsList.Count < tournament.MinTeams)
            throw new ForbiddenException($"At least {tournament.MinTeams} approved teams are required to generate fixtures");

        var matches = new List<Match>();
        var matchDate = generateDto.StartDate;

        for (int i = 0; i < teamsList.Count; i++)
        {
            for (int j = i + 1; j < teamsList.Count; j++)
            {
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    TournamentId = generateDto.TournamentId,
                    HomeTeamId = teamsList[i].TeamId,
                    AwayTeamId = teamsList[j].TeamId,
                    MatchDate = matchDate,
                    Status = "Scheduled",
                };

                matches.Add(match);
                matchDate = matchDate.AddDays(generateDto.DaysBetweenMatches);
            }
        }

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
                        HomeTeamId = teamsList[j].TeamId, // Swapped home and away
                        AwayTeamId = teamsList[i].TeamId,
                        MatchDate = matchDate,
                        Status = "Scheduled",
                    };

                    matches.Add(returnMatch);
                    matchDate = matchDate.AddDays(generateDto.DaysBetweenMatches);
                }
            }
        }

        var createdMatches = new List<Match>();
        foreach (var match in matches)
        {
            var createdMatch = await _matchRepository.CreateAsync(match);
            createdMatches.Add(createdMatch);
        }

        return createdMatches.Select(m => m.ToDto());
    }

    public async Task<MatchDto> StartMatchAsync(Guid matchId, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");


        match.Status = "In Progress";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToDto();
    }

    public async Task<MatchDto> EndMatchAsync(Guid matchId, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");


        match.Status = "Completed";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToDto();
    }

    public async Task<MatchDto> CancelMatchAsync(Guid matchId, string reason, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");


        match.Status = "Cancelled";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToDto();
    }

    public async Task<MatchDto> PostponeMatchAsync(Guid matchId, DateTime newDate, string reason, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");


        match.MatchDate = newDate;
        match.Status = "Postponed";

        var updatedMatch = await _matchRepository.UpdateAsync(match);
        return updatedMatch.ToDto();
    }


    private async Task ValidateTournamentOrganizerAsync(Guid tournamentId, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        if (tournament.OrganizerId != userId)
            throw new UnauthorizedException("Only tournament organizers can perform this action");
    }

    private async Task ValidateMatchEventPermissionsAsync(Guid matchId, Guid userId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");

        await ValidateTournamentOrganizerAsync(match.TournamentId, userId);
    }

    private async Task ValidatePlayerTeamRelationshipAsync(Guid playerId, Guid teamId, Guid matchId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new NotFoundException($"Player {playerId} not found");

        if (player.TeamId != teamId)
            throw new ValidationException($"Player {playerId} does not belong to team {teamId}");

        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");

        if (teamId != match.HomeTeamId && teamId != match.AwayTeamId)
            throw new ValidationException($"Team {teamId} is not playing in this match");
    }



    public async Task<IEnumerable<MatchEventDto>> GetMatchEventsAsync(Guid matchId)
    {
        var cacheKey = $"match_events_{matchId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var events = await _matchEventRepository.GetByMatchIdAsync(matchId);
            return events.Select(e => e.ToDto());
        }, options);
    }

    public async Task<IEnumerable<MatchEventDto>> GetPlayerEventsAsync(Guid playerId)
    {
        var cacheKey = $"player_events_{playerId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var events = await _matchEventRepository.GetByPlayerIdAsync(playerId);
            return events.Select(e => e.ToDto());
        }, options);
    }

    public async Task<IEnumerable<MatchEventDto>> GetTeamEventsAsync(Guid teamId)
    {
        var cacheKey = $"team_events_{teamId}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var events = await _matchEventRepository.GetByTeamIdAsync(teamId);
            return events.Select(e => e.ToDto());
        }, options);
    }

    public async Task<MatchEventDto> GetMatchEventByIdAsync(Guid id)
    {
        var cacheKey = $"match_event_{id}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var matchEvent = await _matchEventRepository.GetByIdAsync(id);
            if (matchEvent == null)
                throw new NotFoundException("Match event not found");

            return matchEvent.ToDto();
        }, options);
    }

    public async Task<MatchEventDto> CreateMatchEventAsync(CreateMatchEventDto createDto, Guid userId)
    {
        await ValidateMatchEventPermissionsAsync(createDto.MatchId, userId);

        await ValidatePlayerTeamRelationshipAsync(createDto.PlayerId, createDto.TeamId, createDto.MatchId);

        var matchEvent = createDto.ToEntity();
        var createdEvent = await _matchEventRepository.CreateAsync(matchEvent);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(createDto.MatchId);

        return createdEvent.ToDto();
    }

    public async Task<MatchEventDto> UpdateMatchEventAsync(Guid id, UpdateMatchEventDto updateDto, Guid userId)
    {
        var existingEvent = await _matchEventRepository.GetByIdAsync(id);
        if (existingEvent == null)
            throw new NotFoundException("Match event not found");

        await ValidateMatchEventPermissionsAsync(existingEvent.MatchId, userId);

        if (updateDto.EventType.HasValue)
            existingEvent.EventType = updateDto.EventType.Value.ToString();

        if (updateDto.Minute.HasValue)
            existingEvent.Minute = updateDto.Minute.Value;

        if (updateDto.PlayerId.HasValue)
        {
            await ValidatePlayerTeamRelationshipAsync(updateDto.PlayerId.Value, existingEvent.TeamId, existingEvent.MatchId);
            existingEvent.PlayerId = updateDto.PlayerId.Value;
        }

        var updatedEvent = await _matchEventRepository.UpdateAsync(existingEvent);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(existingEvent.MatchId);

        return updatedEvent.ToDto();
    }

    public async Task DeleteMatchEventAsync(Guid id, Guid userId)
    {
        var existingEvent = await _matchEventRepository.GetByIdAsync(id);
        if (existingEvent == null)
            throw new NotFoundException("Match event not found");

        await ValidateMatchEventPermissionsAsync(existingEvent.MatchId, userId);

        await _matchEventRepository.DeleteAsync(id);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(existingEvent.MatchId);
    }


    public async Task<FixtureGenerationStatusDto> GetFixtureGenerationStatusAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        return new FixtureGenerationStatusDto
        {
            TournamentId = tournamentId,
            TournamentName = tournament.Name,
            RequiredTeams = tournament.MinTeams,
            MaxTeams = tournament.MaxTeams,
            Status = tournament.Status,
        };
    }
}
