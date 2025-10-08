using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.Service.Implementations;

public class TournamentValidationService(
    ITournamentRepository tournamentRepository) : ITournamentValidationService
{
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;

    public async Task<Tournament> ValidateTournamentExistsAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new NotFoundException($"Tournament not found");

        return tournament;
    }

    public async Task<Tournament> ValidateCanUpdateAsync(Guid tournamentId, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to update this tournament");

        if (tournament.Status == (int)TournamentStatus.InProgress || tournament.Status == (int)TournamentStatus.Completed)
            throw new ForbiddenException("Cannot update tournament that is in progress or completed");

        return tournament;
    }

    public async Task<Tournament> ValidateCanDeleteAsync(Guid tournamentId, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to delete this tournament");

        if (tournament.Status == (int)TournamentStatus.InProgress)
            throw new ForbiddenException("Cannot delete tournament that is in progress");

        return tournament;
    }

    public async Task<Tournament> ValidateCanManageTeamsAsync(Guid tournamentId, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to manage teams for this tournament");

        return tournament;
    }

    public async Task<Tournament> ValidateCanManageMatchesAsync(Guid tournamentId, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to manage matches for this tournament");

        return tournament;
    }

    public async Task<Tournament> ValidateCanManageTournamentAsync(Guid tournamentId, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to manage this tournament");

        return tournament;
    }


    public async Task<Tournament> ValidateTeamCanRegisterAsync(Guid tournamentId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.Status == (int)TournamentStatus.Completed)
            throw new ForbiddenException("Tournament registration is closed");

        if (tournament.Status == (int)TournamentStatus.InProgress)
            throw new ForbiddenException("Tournament has already started");

        return tournament;

    }

    public async Task<Tournament> ValidateCanUpdateStatusAsync(Guid tournamentId, Guid userId, TournamentStatus newStatus)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to update tournament status");

        if (newStatus == TournamentStatus.InProgress && tournament.Status == (int)TournamentStatus.Completed)
            throw new ForbiddenException("Cannot change status from completed to in progress");

        if (newStatus == TournamentStatus.Completed && tournament.Status != (int)TournamentStatus.InProgress)
            throw new ForbiddenException("Can only mark tournament as completed if it is in progress");

        return tournament;
    }
}


public class TeamValidationService(
    ITeamRepository teamRepository,
    ITournamentTeamRepository tournamentTeamRepository) : ITeamValidationService
{
    private readonly ITeamRepository _teamRepository = teamRepository;
    ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;

    public async Task<Team> ValidateTeamExistsAsync(Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(userId);
        if (team == null)
            throw new NotFoundException($"Team not found");

        return team;
    }

    public async Task<Team> ValidateCanManageTeamAsync(Guid userId, Guid teamId)
    {
        var team = await ValidateTeamExistsAsync(userId);

        if (team.UserId != userId)
            throw new ForbiddenException("You don't have permission to manage this teams");

        return team;
    }

    public async Task<Team> ValidateTeamCanBeDeleted(Guid teamId, Guid userId)
    {
        var team = await ValidateCanManageTeamAsync(userId, teamId);
        var tournaments = await _tournamentTeamRepository.GetTournamentsByTeamAsync(team.Id);
        var activeTournaments = tournaments.Where(t => t.Status != (int)TournamentStatus.Completed).ToList();

        if (activeTournaments.Any())
        {
            var tournamentNames = string.Join(", ", activeTournaments.Select(t => t.Name));
            throw new ForbiddenException($"Cannot delete team. Team is registered in tournaments: {tournamentNames}");
        }

        return team;
    }

    public async Task ValidateUserTeamNameUniqueness(string teamName, Guid managerId)
    {
        var existingTeams = await _teamRepository.GetByUserAsync(managerId);
        if (existingTeams.Any(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException("A team with this name is already available");
        }
    }
}

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