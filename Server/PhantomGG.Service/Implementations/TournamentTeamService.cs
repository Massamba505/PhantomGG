using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations;

public class TournamentTeamService(
    ITournamentTeamRepository tournamentTeamRepository,
    ITournamentValidationService validationService,
    ITeamValidationService teamValidationService) : ITournamentTeamService
{
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;
    private readonly ITournamentValidationService _validationService = validationService;
    private readonly ITeamValidationService _teamValidationService = teamValidationService;

    public async Task<IEnumerable<TournamentTeamDto>> GetTeamsAsync(Guid tournamentId, TeamRegistrationStatus? status = null)
    {
        if (status.HasValue)
        {
            var teamsWithStatus = await _tournamentTeamRepository.GetByTournamentAndStatusAsync(tournamentId, status.Value.ToString());
            return teamsWithStatus.Select(tt => tt.ToDto());
        }

        var allTeams = await _tournamentTeamRepository.GetByTournamentAsync(tournamentId);
        return allTeams.Select(tt => tt.ToDto());
    }

    public async Task<bool> IsTeamRegisteredAsync(Guid tournamentId, Guid teamId)
    {
        return await _tournamentTeamRepository.IsTeamRegisteredAsync(tournamentId, teamId);
    }

    public async Task RegisterTeamAsync(Guid tournamentId, Guid teamId, Guid userId)
    {
        await _validationService.ValidateTeamCanRegisterAsync(tournamentId);
        await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);

        if (await _tournamentTeamRepository.IsTeamRegisteredAsync(tournamentId, teamId))
        {
            throw new ConflictException("Team is already registered for this tournament");
        }

        var registration = new TournamentTeam
        {
            TournamentId = tournamentId,
            TeamId = teamId,
            Status = TeamRegistrationStatus.Pending.ToString(),
            RequestedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        //Todo: send email to Organizer

        await _tournamentTeamRepository.CreateAsync(registration);
    }

    public async Task ManageTeamAsync(Guid tournamentId, Guid teamId, TeamAction action, Guid userId)
    {
        await _validationService.ValidateCanManageTeamsAsync(tournamentId, userId);

        var registration = await _tournamentTeamRepository.GetRegistrationAsync(tournamentId, teamId);
        if (registration == null)
        {
            throw new NotFoundException("Team is not registered");
        }

        switch (action)
        {
            case TeamAction.Approve:
                if (registration.Status != TeamRegistrationStatus.Pending.ToString())
                {
                    throw new ForbiddenException("Only pending registrations can be approved");
                }
                registration.Status = TeamRegistrationStatus.Approved.ToString();
                registration.AcceptedAt = DateTime.UtcNow;
                //Todo: send email to Team
                await _tournamentTeamRepository.UpdateAsync(registration);
                break;

            case TeamAction.Reject:
                if (registration.Status != TeamRegistrationStatus.Pending.ToString())
                {
                    throw new ForbiddenException("Only pending registrations can be rejected");
                }
                registration.Status = TeamRegistrationStatus.Rejected.ToString();
                await _tournamentTeamRepository.UpdateAsync(registration);
                //Todo: send email to Team
                break;

            case TeamAction.Withdraw:
                await _tournamentTeamRepository.DeleteAsync(registration);
                break;

            default:
                throw new ValidationException($"Unknown team action: {action}");
        }
    }

    public async Task RemoveTeamAsync(Guid tournamentId, Guid teamId, Guid userId)
    {
        await _validationService.ValidateCanManageTeamsAsync(tournamentId, userId);

        var registration = await _tournamentTeamRepository.GetRegistrationAsync(tournamentId, teamId);
        if (registration == null)
        {
            throw new NotFoundException("Team registration not found");
        }

        await _tournamentTeamRepository.DeleteAsync(registration);
    }

    public async Task<int> GetTeamCountAsync(Guid tournamentId, TeamRegistrationStatus status)
    {
        return await _tournamentTeamRepository.GetCountByStatusAsync(tournamentId, status.ToString());
    }
}
