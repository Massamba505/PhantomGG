using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Validation.Implementations;

public class TournamentValidationService(
    ITournamentRepository tournamentRepository,
    ITournamentTeamRepository tournamentTeamRepository) : ITournamentValidationService
{
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;

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

    public async Task ValidateMinimumTeamsAsync(Guid tournamentId, int minTeams = 2)
    {
        var teams = await _tournamentTeamRepository.GetByTournamentAndStatusAsync(tournamentId, (int)TeamRegistrationStatus.Approved);
        if (teams.Count() < minTeams)
            throw new ValidationException($"Tournament must have at least {minTeams} approved teams to start");
    }

    public async Task ValidateMaximumTeamsAsync(Guid tournamentId, int maxTeams = 32)
    {
        var teams = await _tournamentTeamRepository.GetByTournamentAndStatusAsync(tournamentId, (int)TeamRegistrationStatus.Approved);
        if (teams.Count() >= maxTeams)
            throw new ValidationException($"Tournament has reached the maximum limit of {maxTeams} teams");
    }

    public Task ValidateTournamentDatesAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate <= DateTime.UtcNow)
            throw new ValidationException("Tournament start date must be in the future");

        if (endDate <= startDate)
            throw new ValidationException("Tournament end date must be after start date");

        var duration = (endDate - startDate).TotalDays;
        if (duration > 365)
            throw new ValidationException("Tournament duration cannot exceed 365 days");

        if (duration < 1)
            throw new ValidationException("Tournament must last at least 1 day");

        return Task.CompletedTask;
    }

    public async Task ValidateCanStartTournamentAsync(Guid tournamentId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.Status != (int)TournamentStatus.RegistrationClosed)
            throw new ForbiddenException("Tournament registration must be closed before starting");

        await ValidateMinimumTeamsAsync(tournamentId, 2);

        if (tournament.StartDate > DateTime.UtcNow)
            throw new ForbiddenException("Cannot start tournament before its scheduled start date");
    }

    public async Task ValidateRegistrationStatusAsync(Guid tournamentId, bool shouldBeOpen)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        var isOpen = tournament.Status == (int)TournamentStatus.RegistrationOpen;

        if (shouldBeOpen && !isOpen)
            throw new ValidationException("Tournament registration is not open");

        if (!shouldBeOpen && isOpen)
            throw new ValidationException("Tournament registration is still open");
    }
}
