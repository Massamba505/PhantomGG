using PhantomGG.Service.Interfaces;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Specifications;
using PhantomGG.Service.Exceptions;
using PhantomGG.Repository.Entities;
using PhantomGG.Service.Mappings;
using PhantomGG.Common.Enums;
using Microsoft.Extensions.Caching.Hybrid;

namespace PhantomGG.Service.Implementations;

public class MatchService(
    IMatchRepository matchRepository,
    ITournamentTeamRepository tournamentTeamRepository,
    IMatchValidationService matchValidationService,
    ICacheInvalidationService cacheInvalidationService,
    HybridCache cache) : IMatchService
{
    private readonly IMatchRepository _matchRepository = matchRepository;
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;
    private readonly IMatchValidationService _matchValidationService = matchValidationService;
    private readonly ICacheInvalidationService _cacheInvalidationService = cacheInvalidationService;
    private readonly HybridCache _cache = cache;

    public async Task<MatchDto> GetByIdAsync(Guid id)
    {
        var match = await _matchValidationService.ValidateMatchExistsAsync(id);
        return match.ToDto();
    }

    public async Task<IEnumerable<MatchDto>> GetByTournamentAsync(Guid tournamentId)
    {
        var matches = await _matchRepository.GetByTournamentAsync(tournamentId);
        return matches.Select(m => m.ToDto());
    }

    public async Task<IEnumerable<MatchDto>> GetByTournamentAndStatusAsync(Guid tournamentId, MatchStatus? status)
    {
        if (status.HasValue)
        {
            var matches = await _matchRepository.GetByTournamentAndStatusAsync(tournamentId, status.Value.ToString());
            return matches.Select(m => m.ToDto());
        }

        var allmMtches = await _matchRepository.GetByTournamentAsync(tournamentId);
        return allmMtches.Select(m => m.ToDto());
    }

    public async Task<MatchDto> UpdateResultAsync(Guid matchId, MatchResultDto resultDto, Guid organizerId)
    {
        var match = await _matchValidationService.ValidateCanUpdateResultAsync(matchId, organizerId);

        match.HomeScore = resultDto.HomeScore;
        match.AwayScore = resultDto.AwayScore;
        match.Status = Common.Enums.MatchStatus.Completed.ToString();

        var updatedMatch = await _matchRepository.UpdateAsync(match);

        await _cacheInvalidationService.InvalidateMatchCacheAsync(matchId);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(match.TournamentId);
        return updatedMatch.ToDto();
    }

    public async Task<PagedResult<MatchDto>> SearchAsync(MatchQuery query)
    {
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

        var cacheKey = spec.GetDeterministicKey();

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var result = await _matchRepository.SearchAsync(spec);

                return new PagedResult<MatchDto>(
                    result.Data.Select(m => m.ToDto()),
                    result.Meta.Page,
                    result.Meta.PageSize,
                    result.Meta.TotalRecords
                );
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(2)
            }
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

        var approvedTournamentTeams = await _tournamentTeamRepository.GetByTournamentAndStatusAsync(tournamentId, TeamRegistrationStatus.Approved.ToString());
        var teams = approvedTournamentTeams.Select(tt => tt.Team).ToList();

        if (teams.Count < 2)
        {
            throw new ValidationException("Tournament must have at least 2 approved teams to generate fixtures");
        }

        var existingMatches = await _matchRepository.GetByTournamentAsync(tournamentId);
        if (existingMatches.Any())
        {
            throw new ValidationException("Tournament already has fixtures generated");
        }

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
