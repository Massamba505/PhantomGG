using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Validation.Implementations;

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
