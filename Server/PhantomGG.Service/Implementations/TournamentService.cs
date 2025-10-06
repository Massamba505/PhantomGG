using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.TournamentStanding;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations;

public class TournamentService(
    ITournamentRepository tournamentRepository,
    ITeamService teamService,
    IImageService imageService,
    HybridCache cache) : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITeamService _teamService = teamService;
    private readonly IImageService _imageService = imageService;
    private readonly HybridCache _cache = cache;

    public async Task<PaginatedResponse<TournamentDto>> SearchAsync(TournamentSearchDto searchDto, Guid? userId = null)
    {
        if (userId.HasValue)
        {
            var myTournaments = await _tournamentRepository.SearchAsync(searchDto, organizerId: userId.Value);
            return new PaginatedResponse<TournamentDto>(
                myTournaments.Items.Select(t => t.ToDto()),
                searchDto.PageNumber,
                searchDto.PageSize,
                totalRecords: myTournaments.TotalRecords
            );
        }

        searchDto.IsPublic = true;
        string cacheKey = $"tournaments_search_{searchDto.GetDeterministicKey()}_{searchDto.PageNumber}_{searchDto.PageSize}";
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel =>
            {
                var publicResult = await _tournamentRepository.SearchAsync(searchDto);
                return new PaginatedResponse<TournamentDto>(
                    publicResult.Items.Select(t => t.ToDto()),
                    searchDto.PageNumber,
                    searchDto.PageSize,
                    totalRecords: publicResult.TotalRecords
                );
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5)
            },
            cancellationToken: CancellationToken.None
        );
    }


    public async Task<TournamentDto> GetByIdAsync(Guid id)
    {
        return await _cache.GetOrCreateAsync(
            $"tournament_{id}",
            async cancel =>
            {
                var tournament = await ValidateTournamentExistsAsync(id);
                return tournament.ToDto();
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10)
            },
            cancellationToken: CancellationToken.None
        );
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId)
    {
        ValidateCreateTournament(createDto);

        var tournament = createDto.ToEntity(organizerId);
        await _tournamentRepository.CreateAsync(tournament);

        if (createDto.BannerUrl != null)
        {
            tournament.BannerUrl = await UploadImageAsync(tournament, createDto.BannerUrl);
        }

        if (createDto.LogoUrl != null)
        {
            tournament.LogoUrl = await UploadLogoImageAsync(tournament, createDto.LogoUrl);
        }

        if (createDto.BannerUrl != null || createDto.LogoUrl != null)
        {
            await _tournamentRepository.UpdateAsync(tournament);
        }

        return tournament.ToDto();
    }

    public async Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid organizerId)
    {
        var tournament = await ValidateOrganizerAccessAsync(id, organizerId);

        updateDto.UpdateEntity(tournament);
        if (updateDto.BannerUrl != null)
        {
            tournament.BannerUrl = await UploadImageAsync(tournament, updateDto.BannerUrl);
        }

        if (updateDto.LogoUrl != null)
        {
            tournament.LogoUrl = await UploadLogoImageAsync(tournament, updateDto.LogoUrl);
        }

        await _tournamentRepository.UpdateAsync(tournament);
        return tournament.ToDto();
    }

    public async Task DeleteAsync(Guid id, Guid organizerId)
    {
        var tournament = await ValidateOrganizerAccessAsync(id, organizerId);

        var teamCount = await _tournamentRepository.GetTeamCountAsync(id, TeamRegistrationStatus.Approved);
        if (teamCount > 0 && tournament.Status != TournamentStatus.Completed.ToString())
        {
            throw new ForbiddenException("Cannot delete tournament with registered teams");
        }

        await _tournamentRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TournamentTeamDto>> GetTournamentTeamsAsync(Guid tournamentId, Guid? userId = null, TeamRegistrationStatus status = TeamRegistrationStatus.Approved)
    {
        if (status == TeamRegistrationStatus.Approved && !userId.HasValue)
        {
            return await _cache.GetOrCreateAsync(
                $"tournament_teams_{tournamentId}_{status}",
                async cancel =>
                {
                    var tournament = await ValidateTournamentExistsAsync(tournamentId);
                    IEnumerable<TournamentTeam> tournamentTeams = await _tournamentRepository.GetTournamentTeamByStatus(tournamentId, status);
                    return tournamentTeams.Select(tt => tt.ToDto());
                },
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(15)
                },
                cancellationToken: CancellationToken.None
            );
        }

        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (userId.HasValue && tournament.OrganizerId != userId.Value && status != TeamRegistrationStatus.Approved)
        {
            throw new ForbiddenException("Only organizers can view teams with non approved status");
        }

        IEnumerable<TournamentTeam> tournamentTeams = await _tournamentRepository.GetTournamentTeamByStatus(tournamentId, status);
        return tournamentTeams.Select(tt => tt.ToDto());
    }

    public async Task UpdateTournamentStatusAsync(Guid tournamentId, Guid userId, TournamentStatus status)
    {
        var tournament = await ValidateOrganizerAccessAsync(tournamentId, userId);
        tournament.Status = status.ToString();
        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task<IEnumerable<MatchDto>> GetTournamentMatchesAsync(Guid tournamentId, Guid? userId = null)
    {
        return await _cache.GetOrCreateAsync(
            $"tournament_matches_{tournamentId}",
            async cancel =>
            {
                var tournament = await ValidateTournamentExistsAsync(tournamentId);
                var matches = await _tournamentRepository.GetTournamentMatchesAsync(tournamentId);
                return matches.Select(m => m.ToDto());
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10)
            },
            cancellationToken: CancellationToken.None
        );
    }

    public async Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId)
    {
        await ValidateTournamentExistsAsync(tournamentId);
        return new List<TournamentStandingDto>();
    }

    public async Task ManageTeamAsync(Guid tournamentId, Guid? teamId, TeamActionDto actionDto, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        switch (actionDto.Action)
        {
            case TeamAction.Register:
                if (!actionDto.TeamId.HasValue)
                {
                    throw new ValidationException("TeamId is required for register action");
                }
                await RegisterTeamInternal(tournament.Id, actionDto.TeamId.Value, userId);
                break;

            case TeamAction.Withdraw:
                if (!actionDto.TeamId.HasValue)
                {
                    throw new ValidationException("TeamId is required for withdraw action");
                }
                await WithdrawTeamInternal(tournament.Id, actionDto.TeamId.Value, userId);
                break;

            case TeamAction.Approve:
                if (!teamId.HasValue)
                {
                    throw new ValidationException("TeamId is required for approve action");
                }
                await ApproveTeamInternal(tournament.Id, teamId.Value, userId);
                break;

            case TeamAction.Reject:
                if (!teamId.HasValue)
                {
                    throw new ValidationException("TeamId is required for reject action");
                }
                await RejectTeamInternal(tournament.Id, teamId.Value, userId);
                break;

            default:
                throw new ValidationException($"Unknown team action: {actionDto.Action}");
        }
    }

    private async Task RegisterTeamInternal(Guid tournamentId, Guid teamId, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.Status == TournamentStatus.RegistrationClosed.ToString())
        {
            throw new ForbiddenException("Tournament registration is closed");
        }

        var team = await _teamService.GetByIdAsync(teamId);

        if (team.UserId != userId)
        {
            throw new UnauthorizedException("You don't have permission to register this team");
        }

        if (await _tournamentRepository.IsTeamRegisteredAsync(tournamentId, team.Id))
        {
            throw new ForbiddenException("Team is already registered for this tournament");
        }

        var approvedCount = await _tournamentRepository.GetTeamCountAsync(tournamentId, TeamRegistrationStatus.Approved);
        if (approvedCount >= tournament.MaxTeams)
        {
            throw new ForbiddenException("Tournament has reached maximum team capacity");
        }

        var registration = new TournamentTeam
        {
            TournamentId = tournamentId,
            TeamId = team.Id,
            Status = TeamRegistrationStatus.Pending.ToString(),
            RequestedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _tournamentRepository.RegisterTeamForTournamentAsync(registration);
    }

    private async Task WithdrawTeamInternal(Guid tournamentId, Guid teamId, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.Status == TournamentStatus.InProgress.ToString())
        {
            throw new ForbiddenException("Cannot withdraw from tournament that is in progress");
        }

        var team = await _teamService.GetByIdAsync(teamId);

        if (team.UserId != userId && tournament.OrganizerId != userId)
        {
            throw new UnauthorizedException("You don't have permission to withdraw this team");
        }

        if (!await _tournamentRepository.IsTeamRegisteredAsync(tournamentId, team.Id))
        {
            throw new ForbiddenException("Team is not registered for this tournament");
        }

        var registration = await _tournamentRepository.GetTeamRegistrationAsync(tournament.Id, team.Id);
        if (registration == null)
        {
            throw new NotFoundException("Team registration not found");
        }

        await _tournamentRepository.RemoveTeamFromTournamentAsync(registration);
    }

    private async Task ApproveTeamInternal(Guid tournamentId, Guid teamId, Guid userId)
    {
        var tournament = await ValidateOrganizerAccessAsync(tournamentId, userId);

        var registration = await _tournamentRepository.GetTeamRegistrationAsync(tournamentId, teamId);
        if (registration == null)
        {
            throw new NotFoundException("Team registration not found");
        }

        if (registration.Status != TeamRegistrationStatus.Pending.ToString())
        {
            throw new ForbiddenException("Only pending registrations can be approved");
        }

        var approvedCount = await _tournamentRepository.GetTeamCountAsync(tournamentId, TeamRegistrationStatus.Approved);
        if (approvedCount >= tournament.MaxTeams)
        {
            throw new ForbiddenException("Tournament has reached maximum team capacity");
        }

        await _tournamentRepository.ChangeTeamRegistrationStatusAsync(registration, TeamRegistrationStatus.Approved);
    }

    private async Task RejectTeamInternal(Guid tournamentId, Guid teamId, Guid userId)
    {
        var tournament = await ValidateOrganizerAccessAsync(tournamentId, userId);

        var registration = await _tournamentRepository.GetTeamRegistrationAsync(tournamentId, teamId);
        if (registration == null)
        {
            throw new NotFoundException("Team registration not found");
        }

        if (registration.Status != TeamRegistrationStatus.Pending.ToString())
        {
            throw new ForbiddenException("Only pending registrations can be rejected");
        }

        await _tournamentRepository.ChangeTeamRegistrationStatusAsync(registration, TeamRegistrationStatus.Rejected);
    }

    public async Task CreateTournamentBracketAsync(Guid tournamentId, Guid organizerId)
    {
        await ValidateOrganizerAccessAsync(tournamentId, organizerId);
        throw new NotImplementedException("Tournament bracket creation not yet implemented");
    }

    public async Task<string> UploadImageAsync(Tournament tournament, IFormFile file)
    {
        if (!string.IsNullOrEmpty(tournament.BannerUrl))
        {
            await _imageService.DeleteImageAsync(tournament.BannerUrl);
        }

        var bannerUrl = await _imageService.SaveImageAsync(file, ImageType.TournamentBanner, tournament.Id);

        return bannerUrl;
    }

    public async Task<string> UploadLogoImageAsync(Tournament tournament, IFormFile file)
    {
        if (!string.IsNullOrEmpty(tournament.LogoUrl))
        {
            await _imageService.DeleteImageAsync(tournament.LogoUrl);
        }

        var logoUrl = await _imageService.SaveImageAsync(file, ImageType.TournamentLogo, tournament.Id);

        return logoUrl;
    }

    private async Task<Tournament> ValidateTournamentExistsAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");
        return tournament;
    }

    private async Task<Tournament> ValidateOrganizerAccessAsync(Guid tournamentId, Guid organizerId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);
        if (tournament.OrganizerId != organizerId)
        {
            throw new ForbiddenException("You don't have permission to access this tournament");
        }
        return tournament;
    }

    private static void ValidateCreateTournament(CreateTournamentDto createDto)
    {
        if (createDto.StartDate <= DateTime.UtcNow)
            throw new ValidationException("Tournament start date must be in the future");

        if (createDto.EndDate <= createDto.StartDate)
            throw new ValidationException("Tournament end date must be after start date");

        if (createDto.RegistrationDeadline >= createDto.StartDate)
            throw new ValidationException("Registration deadline must be before tournament start date");
    }
}