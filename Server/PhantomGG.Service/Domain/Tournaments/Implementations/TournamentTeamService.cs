using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Mappings;
using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Service.Validation.Interfaces;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Infrastructure.Email.Interfaces;
using PhantomGG.Service.Domain.Tournaments.Interfaces;

namespace PhantomGG.Service.Domain.Tournaments.Implementations;

public class TournamentTeamService(
    ITournamentTeamRepository tournamentTeamRepository,
    ITournamentValidationService validationService,
    ITeamValidationService teamValidationService,
    ICacheInvalidationService cacheInvalidationService,
    ITeamRepository teamRepository,
    ITournamentRepository tournamentRepository,
    IUserRepository userRepository,
    IEmailService emailService,
    HybridCache cache) : ITournamentTeamService
{
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;
    private readonly ITournamentValidationService _validationService = validationService;
    private readonly ITeamValidationService _teamValidationService = teamValidationService;
    private readonly ICacheInvalidationService _cacheInvalidationService = cacheInvalidationService;
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly HybridCache _cache = cache;

    public async Task<IEnumerable<TournamentTeamDto>> GetTeamsAsync(Guid tournamentId, TeamRegistrationStatus? status = null)
    {
        await _validationService.ValidateTournamentExistsAsync(tournamentId);

        var cacheKey = status.HasValue
            ? $"tournament_teams_{tournamentId}_{status}"
            : $"tournament_teams_{tournamentId}_all";

        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            if (status.HasValue)
            {
                var teamsWithStatus = await _tournamentTeamRepository.GetByTournamentAndStatusAsync(tournamentId, (int)status);
                return teamsWithStatus.Select(tt => tt.ToDto());
            }

            var allTeams = await _tournamentTeamRepository.GetByTournamentAsync(tournamentId);
            return allTeams.Select(tt => tt.ToDto());
        }, options);
    }

    public async Task<IEnumerable<TournamentTeamDto>> GetOrganizerPendingApprovalsAsync(Guid organizerId)
    {
        var organizerTournaments = await _tournamentRepository.GetByOrganizerAsync(organizerId);
        var tournamentIds = organizerTournaments.Select(t => t.Id).ToList();

        if (!tournamentIds.Any())
        {
            return [];
        }

        var allPendingApprovals = new List<TournamentTeam>();
        foreach (var tournamentId in tournamentIds)
        {
            var pendingTeams = await _tournamentTeamRepository.GetByTournamentAndStatusAsync(
                tournamentId,
                (int)TeamRegistrationStatus.Pending
            );
            allPendingApprovals.AddRange(pendingTeams);
        }

        return allPendingApprovals.Select(tt => tt.ToDto());
    }

    public async Task<bool> IsTeamRegisteredAsync(Guid tournamentId, Guid teamId)
    {
        await _validationService.ValidateTournamentExistsAsync(tournamentId);

        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
        {
            throw new NotFoundException("Team not found");
        }

        return await _tournamentTeamRepository.IsTeamRegisteredAsync(tournamentId, teamId);
    }

    public async Task RegisterTeamAsync(Guid tournamentId, Guid teamId, Guid userId)
    {
        await _validationService.ValidateTeamCanRegisterAsync(tournamentId);

        await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);

        await _teamValidationService.ValidateMinimumPlayersForTournamentAsync(teamId, 5);
        await _teamValidationService.ValidateTeamHasRequiredPositionsAsync(teamId);

        if (await _tournamentTeamRepository.IsTeamRegisteredAsync(tournamentId, teamId))
        {
            throw new ConflictException("Team is already registered for this tournament");
        }

        await _validationService.ValidateMaximumTeamsAsync(tournamentId);

        var registration = new TournamentTeam
        {
            TournamentId = tournamentId,
            TeamId = teamId,
            Status = (int)TeamRegistrationStatus.Pending,
            RequestedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _tournamentTeamRepository.CreateAsync(registration);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournamentId);

        try
        {
            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            var team = await _teamRepository.GetByIdAsync(teamId);
            var organizer = await _userRepository.GetByIdAsync(tournament!.OrganizerId);

            if (organizer != null && tournament != null && team != null)
            {
                await _emailService.SendTeamRegistrationRequestAsync(
                    organizer.Email,
                    organizer.FirstName,
                    team.Name,
                    tournament.Name);
            }
        }
        catch (Exception)
        {
        }
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
                if (registration.Status != (int)TeamRegistrationStatus.Pending)
                {
                    throw new ForbiddenException("Only pending registrations can be approved");
                }

                await _validationService.ValidateMaximumTeamsAsync(tournamentId);

                await _teamValidationService.ValidateMinimumPlayersForTournamentAsync(teamId, 5);
                await _teamValidationService.ValidateTeamHasRequiredPositionsAsync(teamId);

                registration.Status = (int)TeamRegistrationStatus.Approved;
                registration.AcceptedAt = DateTime.UtcNow;
                await _tournamentTeamRepository.UpdateAsync(registration);
                await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournamentId);

                try
                {
                    var tournamentData = await _tournamentRepository.GetByIdAsync(tournamentId);
                    var teamData = await _teamRepository.GetByIdAsync(teamId);
                    var teamManager = teamData != null ? await _userRepository.GetByIdAsync(teamData.UserId) : null;

                    if (teamManager != null && tournamentData != null && teamData != null)
                    {
                        await _emailService.SendTeamApprovedAsync(
                            teamManager.Email,
                            teamManager.FirstName,
                            teamData.Name,
                            tournamentData.Name);
                    }
                }
                catch (Exception)
                {
                }
                break;

            case TeamAction.Reject:
                if (registration.Status != (int)TeamRegistrationStatus.Pending)
                {
                    throw new ForbiddenException("Only pending registrations can be rejected");
                }
                registration.Status = (int)TeamRegistrationStatus.Rejected;
                await _tournamentTeamRepository.UpdateAsync(registration);
                await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournamentId);

                try
                {
                    var tournamentData = await _tournamentRepository.GetByIdAsync(tournamentId);
                    var teamData = await _teamRepository.GetByIdAsync(teamId);
                    var teamManager = teamData != null ? await _userRepository.GetByIdAsync(teamData.UserId) : null;

                    if (teamManager != null && tournamentData != null && teamData != null)
                    {
                        await _emailService.SendTeamRejectedAsync(
                            teamManager.Email,
                            teamManager.FirstName,
                            teamData.Name,
                            tournamentData.Name);
                    }
                }
                catch (Exception)
                {
                }
                break;

            case TeamAction.Withdraw:
                var tournament = await _validationService.ValidateTournamentExistsAsync(tournamentId);
                if (tournament.Status == (int)TournamentStatus.InProgress || tournament.Status == (int)TournamentStatus.Completed)
                {
                    throw new ForbiddenException("Cannot withdraw from a tournament that is in progress or completed");
                }

                await _tournamentTeamRepository.DeleteAsync(registration);
                await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournamentId);
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

        var tournament = await _validationService.ValidateTournamentExistsAsync(tournamentId);
        if (tournament.Status == (int)TournamentStatus.InProgress)
        {
            throw new ForbiddenException("Cannot remove teams from a tournament that is in progress");
        }
        if (tournament.Status == (int)TournamentStatus.Completed)
        {
            throw new ForbiddenException("Cannot remove teams from a completed tournament");
        }

        await _tournamentTeamRepository.DeleteAsync(registration);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournamentId);
    }

    public async Task<int> GetTeamCountAsync(Guid tournamentId, TeamRegistrationStatus status)
    {
        return await _tournamentTeamRepository.GetCountByStatusAsync(tournamentId, (int)status);
    }
}
