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

        if (tournament.Status == TournamentStatus.InProgress.ToString() || tournament.Status == TournamentStatus.Completed.ToString())
            throw new ForbiddenException("Cannot update tournament that is in progress or completed");

        return tournament;
    }

    public async Task<Tournament> ValidateCanDeleteAsync(Guid tournamentId, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to delete this tournament");

        if (tournament.Status == TournamentStatus.InProgress.ToString())
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

    public async Task<Tournament> ValidateTeamCanRegisterAsync(Guid tournamentId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.Status == TournamentStatus.Completed.ToString())
            throw new ForbiddenException("Tournament registration is closed");

        if (tournament.Status == TournamentStatus.InProgress.ToString())
            throw new ForbiddenException("Tournament has already started");

        return tournament;

    }

    public async Task<Tournament> ValidateCanUpdateStatusAsync(Guid tournamentId, Guid userId, TournamentStatus newStatus)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to update tournament status");

        if (newStatus == TournamentStatus.InProgress && tournament.Status == TournamentStatus.Completed.ToString())
            throw new ForbiddenException("Cannot change status from completed to in progress");

        if (newStatus == TournamentStatus.Completed && tournament.Status != TournamentStatus.InProgress.ToString())
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
        var activeTournaments = tournaments.Where(t => t.Status != TournamentStatus.Completed.ToString()).ToList();

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