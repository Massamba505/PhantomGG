using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Validation.Implementations;

public class MatchValidationService(
    IMatchRepository matchRepository,
    ITournamentRepository tournamentRepository,
    ITeamRepository teamRepository,
    ITournamentTeamRepository tournamentTeamRepository,
    IPlayerRepository playerRepository) : IMatchValidationService
{
    private readonly IMatchRepository _matchRepository = matchRepository;
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public async Task<Match> ValidateMatchExistsAsync(Guid matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");

        return match;
    }

    public async Task<Match> ValidateCanUpdateMatchAsync(Guid matchId, Guid userId)
    {
        var match = await ValidateMatchExistsAsync(matchId);

        if (match.Tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to update this match");

        if (match.Status == (int)MatchStatus.Completed)
            throw new ForbiddenException("Cannot update completed matches");

        return match;
    }

    public async Task<Match> ValidateCanDeleteMatchAsync(Guid matchId, Guid userId)
    {
        var match = await ValidateMatchExistsAsync(matchId);

        if (match.Tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to delete this match");

        if (match.Status == (int)MatchStatus.InProgress || match.Status == (int)MatchStatus.Completed)
            throw new ForbiddenException("Cannot delete matches that have started or completed");

        return match;
    }

    public async Task<Match> ValidateCanUpdateResultAsync(Guid matchId, Guid userId)
    {
        var match = await ValidateMatchExistsAsync(matchId);

        if (match.Tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to update match results");

        if (match.Status == (int)MatchStatus.Scheduled)
            throw new ForbiddenException("Cannot update results for scheduled matches");

        return match;
    }

    public async Task ValidateTeamsCanPlayAsync(Guid homeTeamId, Guid awayTeamId, Guid tournamentId)
    {
        if (homeTeamId == awayTeamId)
            throw new ValidationException("A team cannot play against itself");

        var homeTeamRegistration = await _tournamentTeamRepository.GetRegistrationAsync(tournamentId, homeTeamId);
        var awayTeamRegistration = await _tournamentTeamRepository.GetRegistrationAsync(tournamentId, awayTeamId);

        if (homeTeamRegistration == null)
            throw new ValidationException("Home team is not registered in this tournament");

        if (awayTeamRegistration == null)
            throw new ValidationException("Away team is not registered in this tournament");

        if (homeTeamRegistration.Status != (int)TeamRegistrationStatus.Approved)
            throw new ValidationException("Home team is not approved in this tournament");

        if (awayTeamRegistration.Status != (int)TeamRegistrationStatus.Approved)
            throw new ValidationException("Away team is not approved in this tournament");
    }

    public async Task ValidateMatchSchedulingAsync(Guid homeTeamId, Guid awayTeamId, DateTime matchDate, Guid? excludeMatchId = null)
    {
        var hasConflictingMatch = await _matchRepository.TeamsHaveMatchOnDateAsync(homeTeamId, awayTeamId, matchDate, excludeMatchId);

        if (hasConflictingMatch)
            throw new ConflictException("Teams already have a match scheduled on this date");

        if (!excludeMatchId.HasValue && matchDate <= DateTime.UtcNow)
            throw new ValidationException("Match date must be in the future");
    }

    public async Task<Tournament> ValidateTournamentForMatchAsync(Guid tournamentId, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to manage matches for this tournament");

        if (tournament.Status == (int)TournamentStatus.Completed)
            throw new ForbiddenException("Cannot manage matches for completed tournaments");

        return tournament;
    }

    public async Task ValidatePlayerTeamRelationshipAsync(Guid playerId, Guid teamId, Guid matchId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new NotFoundException($"Player not found");

        if (player.TeamId != teamId)
            throw new ValidationException($"Player does not belong to the specified team");

        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");

        if (teamId != match.HomeTeamId && teamId != match.AwayTeamId)
            throw new ValidationException($"Team is not playing in this match");
    }
}
