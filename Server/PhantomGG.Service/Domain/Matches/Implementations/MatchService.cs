using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Specifications;
using PhantomGG.Service.Exceptions;
using PhantomGG.Repository.Entities;
using PhantomGG.Service.Mappings;
using PhantomGG.Common.Enums;
using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Service.Validation.Interfaces;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Domain.Matches.Interfaces;

namespace PhantomGG.Service.Domain.Matches.Implementations;

public class MatchService(
    IMatchRepository matchRepository,
    IMatchEventRepository matchEventRepository,
    ITournamentTeamRepository tournamentTeamRepository,
    ITeamRepository teamRepository,
    IMatchValidationService matchValidationService,
    ITournamentValidationService tournamentValidationService,
    ICacheInvalidationService cacheInvalidationService,
    HybridCache cache) : IMatchService
{
    private readonly IMatchRepository _matchRepository = matchRepository;
    private readonly IMatchEventRepository _matchEventRepository = matchEventRepository;
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly IMatchValidationService _matchValidationService = matchValidationService;
    private readonly ITournamentValidationService _tournamentValidationService = tournamentValidationService;
    private readonly ICacheInvalidationService _cacheInvalidationService = cacheInvalidationService;
    private readonly HybridCache _cache = cache;

    public async Task<MatchDto> GetByIdAsync(Guid id)
    {
        var match = await _matchValidationService.ValidateMatchExistsAsync(id);
        return match.ToDto();
    }

    public async Task<IEnumerable<MatchDto>> GetByTournamentAndStatusAsync(Guid tournamentId, MatchStatus? status)
    {
        await _tournamentValidationService.ValidateTournamentExistsAsync(tournamentId);

        if (status.HasValue)
        {
            var matches = await _matchRepository.GetByTournamentAndStatusAsync(tournamentId, (int)status.Value);
            return matches.Select(m => m.ToDto());
        }

        var allmMtches = await _matchRepository.GetByTournamentAsync(tournamentId);
        return allmMtches.Select(m => m.ToDto());
    }

    public async Task<MatchDto> UpdateResultAsync(Guid matchId, MatchResultDto resultDto, Guid organizerId)
    {
        var match = await _matchValidationService.ValidateCanUpdateResultAsync(matchId, organizerId);

        var matchEvents = await _matchEventRepository.GetByMatchIdAsync(matchId);
        var goalEvents = matchEvents.Where(e => e.EventType == (int)MatchEventType.Goal);

        var homeScore = goalEvents.Count(e => e.TeamId == match.HomeTeamId);
        var awayScore = goalEvents.Count(e => e.TeamId == match.AwayTeamId);

        match.HomeScore = homeScore;
        match.AwayScore = awayScore;
        match.Status = (int)resultDto.Status;

        var updatedMatch = await _matchRepository.UpdateAsync(match);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(matchId);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(match.TournamentId);
        return updatedMatch.ToDto();
    }

    public async Task<PagedResult<MatchDto>> SearchAsync(MatchQuery query)
    {
        if (query.TournamentId.HasValue)
        {
            await _tournamentValidationService.ValidateTournamentExistsAsync(query.TournamentId.Value);
        }

        var spec = new MatchSpecification
        {
            SearchTerm = query.Q,
            TournamentId = query.TournamentId,
            TeamId = query.TeamId,
            Status = query.Status,
            DateFrom = query.From,
            DateTo = query.To,
            Page = query.Page,
            PageSize = query.PageSize
        };

        var result = await _matchRepository.SearchAsync(spec);

        return new PagedResult<MatchDto>(
            result.Data.Select(m => m.ToDto()),
            result.Meta.Page,
            result.Meta.PageSize,
            result.Meta.TotalRecords
        );
    }

    public async Task<PagedResult<MatchDto>> GetUserMatchesAsync(MatchQuery query, Guid userId)
    {
        var userTeams = await _teamRepository.GetByUserAsync(userId);
        var teamIds = userTeams.Select(t => t.Id).ToList();

        if (!teamIds.Any())
        {
            return new PagedResult<MatchDto>(
                [],
                query.Page,
                query.PageSize,
                0
            );
        }

        var spec = new MatchSpecification
        {
            SearchTerm = query.Q,
            TournamentId = query.TournamentId,
            UserTeamIds = teamIds,
            Status = query.Status,
            DateFrom = query.From,
            DateTo = query.To,
            Page = query.Page,
            PageSize = query.PageSize
        };

        var result = await _matchRepository.SearchAsync(spec);

        return new PagedResult<MatchDto>(
            result.Data.Select(m => m.ToDto()),
            result.Meta.Page,
            result.Meta.PageSize,
            result.Meta.TotalRecords
        );
    }

    public async Task<MatchDto> CreateAsync(CreateMatchDto createDto, Guid userId)
    {
        var tournament = await _matchValidationService.ValidateTournamentForMatchAsync(createDto.TournamentId, userId);

        await _matchValidationService.ValidateTeamsCanPlayAsync(createDto.HomeTeamId, createDto.AwayTeamId, createDto.TournamentId);

        await _matchValidationService.ValidateMatchSchedulingAsync(createDto.HomeTeamId, createDto.AwayTeamId, createDto.MatchDate);

        var match = createDto.ToEntity();
        var createdMatch = await _matchRepository.CreateAsync(match);

        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(createdMatch.TournamentId);

        return createdMatch.ToDto();
    }

    public async Task<MatchDto> UpdateAsync(Guid id, UpdateMatchDto updateDto, Guid userId)
    {
        var match = await _matchValidationService.ValidateCanUpdateMatchAsync(id, userId);

        if (updateDto.MatchDate != match.MatchDate)
        {
            await _matchValidationService.ValidateMatchSchedulingAsync(
                match.HomeTeamId,
                match.AwayTeamId,
                updateDto.MatchDate,
                id);
        }

        updateDto.UpdateEntity(match);
        var updatedMatch = await _matchRepository.UpdateAsync(match);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(id);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(match.TournamentId);

        return updatedMatch.ToDto();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var match = await _matchValidationService.ValidateCanDeleteMatchAsync(id, userId);

        await _matchRepository.DeleteAsync(id);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(id);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(match.TournamentId);
    }

    public async Task CreateTournamentBracketAsync(Guid tournamentId, Guid organizerId)
    {
        var tournament = await _matchValidationService.ValidateTournamentForMatchAsync(tournamentId, organizerId);

        var approvedTournamentTeams = await _tournamentTeamRepository.GetByTournamentAndStatusAsync(tournamentId, (int)TeamRegistrationStatus.Approved);
        var teams = approvedTournamentTeams.Select(tt => tt.Team).ToList();

        await ValidateMinimumTeamsForFixturesAsync(teams, tournament);
        await ValidateNoExistingFixturesAsync(tournamentId);

        var fixtures = GenerateRoundRobinFixtures(teams, tournamentId);

        foreach (var fixture in fixtures)
        {
            await _matchRepository.CreateAsync(fixture);
        }

        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournamentId);
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
                    Status = (int)MatchStatus.Scheduled,
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

    private Task ValidateMinimumTeamsForFixturesAsync(List<Team> teams, Tournament tournament)
    {
        if (teams.Count < tournament.MinTeams)
        {
            throw new ValidationException($"Tournament must have at least {tournament.MinTeams} approved teams to generate fixtures");
        }
        return Task.CompletedTask;
    }

    private async Task ValidateNoExistingFixturesAsync(Guid tournamentId)
    {
        var existingMatches = await _matchRepository.GetByTournamentAsync(tournamentId);
        if (existingMatches.Any())
        {
            throw new ValidationException("Tournament already has fixtures generated");
        }
    }
}
