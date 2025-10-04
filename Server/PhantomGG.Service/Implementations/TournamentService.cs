using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.TournamentStanding;
using PhantomGG.Models.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations;

public class TournamentService(
    ITournamentRepository tournamentRepository,
    ITeamService teamService,
    IImageService imageService) : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITeamService _teamService = teamService;
    private readonly IImageService _imageService = imageService;

    public async Task<TournamentDto> GetByIdAsync(Guid id)
    {
        var tournament = await ValidateTournamentExistsAsync(id);
        return tournament.ToDto();
    }

    public async Task<PaginatedResponse<TournamentDto>> SearchAsync(TournamentSearchDto searchDto)
    {
        var paginatedResult = await _tournamentRepository.SearchAsync(searchDto);

        return new PaginatedResponse<TournamentDto>(
            paginatedResult.Items.Select(t => t.ToDto()),
            searchDto.PageNumber,
            searchDto.PageSize,
            totalRecords: paginatedResult.TotalRecords
        );
    }

    public async Task<IEnumerable<TournamentTeamDto>> GetTournamentTeamsAsync(Guid tournamentId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);
        var tournamentTeams = await _tournamentRepository.GetTournamentTeamsAsync(tournamentId);
        return tournamentTeams.Select(tt => tt.ToDto());
    }

    public async Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId)
    {
        await ValidateTournamentExistsAsync(tournamentId);

        return new List<TournamentStandingDto>();
    }

    public async Task<IEnumerable<MatchDto>> GetTournamentMatchesAsync(Guid tournamentId)
    {
        await ValidateTournamentExistsAsync(tournamentId);
        return new List<MatchDto>();
    }

    public async Task<PaginatedResponse<TournamentDto>> GetMyTournamentsAsync(TournamentSearchDto searchDto, Guid organizerId)
    {
        var paginatedResult = await _tournamentRepository.SearchAsync(searchDto, organizerId: organizerId);

        return new PaginatedResponse<TournamentDto>(
            paginatedResult.Items.Select(t => t.ToDto()),
            searchDto.PageNumber,
            searchDto.PageSize,
            totalRecords: paginatedResult.TotalRecords
        );
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId)
    {
        ValidateCreateTournament(createDto);

        var tournament = createDto.ToEntity(organizerId);

        // Save tournament first to ensure it has a proper database ID
        await _tournamentRepository.CreateAsync(tournament);

        // Then upload images and update the tournament with the image URLs
        if (createDto.BannerUrl != null)
        {
            tournament.BannerUrl = await UploadImageAsync(tournament, createDto.BannerUrl);
        }

        if (createDto.LogoUrl != null)
        {
            tournament.LogoUrl = await UploadLogoImageAsync(tournament, createDto.LogoUrl);
        }

        // Update tournament with image URLs if any files were uploaded
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

        var teamCount = await _tournamentRepository.GetTournamentTeamCountAsync(id);
        if (teamCount > 0)
        {
            throw new ForbiddenException("Cannot delete tournament with registered teams");
        }

        await _tournamentRepository.DeleteAsync(id);
    }

    public async Task<string> UploadImageAsync(Tournament tournament, IFormFile file)
    {
        if (!string.IsNullOrEmpty(tournament.BannerUrl))
        {
            await _imageService.DeleteImageAsync(tournament.BannerUrl);
        }

        Console.WriteLine($"Uploading banner for tournament ID: {tournament.Id}");
        var bannerUrl = await _imageService.SaveImageAsync(file, ImageType.TournamentBanner, tournament.Id);
        Console.WriteLine($"Generated banner URL: {bannerUrl}");

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

    public async Task RegisterForTournamentAsync(Guid tournamentId, JoinTournamentDto registrationDto, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.Status == TournamentStatus.RegistrationClosed.ToString())
        {
            throw new ForbiddenException("Tournament registration is closed");
        }

        var team = await _teamService.GetByIdAsync(registrationDto.TeamId);

        if (team.UserId != userId)
        {
            throw new UnauthorizedException("You don't have permission to register this team");
        }

        if (await _tournamentRepository.IsTeamRegisteredAsync(tournamentId, team.Id))
        {
            throw new ForbiddenException("Team is already registered for this tournament");
        }

        var approvedCount = await _tournamentRepository.GetApprovedTeamCountAsync(tournamentId);
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

    public async Task WithdrawFromTournamentAsync(Guid tournamentId, LeaveTournamentDto withdrawDto, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (tournament.Status == TournamentStatus.InProgress.ToString())
        {
            throw new ForbiddenException("Cannot withdraw from tournament that is in progress");
        }

        var team = await _teamService.GetByIdAsync(withdrawDto.TeamId);

        if (tournament.OrganizerId != userId)
        {
            throw new UnauthorizedException("You don't have permission to register this team");
        }

        if (!await _tournamentRepository.IsTeamRegisteredAsync(tournamentId, team.Id))
        {
            throw new ForbiddenException("Team is not registered for this tournament");
        }

        await _tournamentRepository.RemoveTeamFromTournamentAsync(tournamentId, team.Id);
    }

    public async Task<IEnumerable<TournamentTeamDto>> GetPendingTeamsAsync(Guid tournamentId, Guid organizerId)
    {
        await ValidateOrganizerAccessAsync(tournamentId, organizerId);
        var tournamentTeams = await _tournamentRepository.GetPendingTeamsAsync(tournamentId);
        return tournamentTeams.Select(tt => tt.ToDto());
    }

    public async Task ApproveTeamAsync(Guid tournamentId, Guid teamId, Guid organizerId)
    {
        var tournament = await ValidateOrganizerAccessAsync(tournamentId, organizerId);

        var registration = await _tournamentRepository.GetTeamRegistrationAsync(tournamentId, teamId);
        if (registration == null)
        {
            throw new NotFoundException("Team registration not found");
        }
        if (registration.Status != TeamRegistrationStatus.Pending.ToString())
        {
            throw new ForbiddenException("Only pending registrations can be approved");
        }

        var approvedCount = await _tournamentRepository.GetApprovedTeamCountAsync(tournamentId);
        if (approvedCount >= tournament.MaxTeams)
        {
            throw new ForbiddenException("Tournament has reached maximum team capacity");
        }

        await _tournamentRepository.ApproveTeamAsync(tournamentId, teamId);
    }

    public async Task RejectTeamAsync(Guid tournamentId, Guid teamId, Guid organizerId)
    {
        var tournament = await ValidateOrganizerAccessAsync(tournamentId, organizerId);

        var registration = await _tournamentRepository.GetTeamRegistrationAsync(tournamentId, teamId);
        if (registration == null)
        {
            throw new NotFoundException("Team registration not found");
        }
        if (registration.Status != TeamRegistrationStatus.Pending.ToString())
        {
            throw new ForbiddenException("Only pending registrations can be rejected");
        }

        await _tournamentRepository.RejectTeamAsync(tournamentId, teamId);
    }

    public async Task CreateTournamentBracketAsync(Guid tournamentId, Guid organizerId)
    {
        await ValidateOrganizerAccessAsync(tournamentId, organizerId);

        // TODO: Implement bracket generation logic
        throw new NotImplementedException("Tournament bracket generation not yet implemented");
    }

    public async Task UpdateMatchResultAsync(Guid tournamentId, Guid matchId, object resultDto, Guid organizerId)
    {
        await ValidateOrganizerAccessAsync(tournamentId, organizerId);

        // TODO: Implement match result update logic
        throw new NotImplementedException("Match result update not yet implemented");
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
            throw new UnauthorizedException("You don't have permission to this tournament");
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

    #region Public Methods (no authentication required)

    public async Task<TournamentDto> GetPublicTournamentByIdAsync(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null || !tournament.IsPublic)
            throw new NotFoundException("Public tournament not found");

        return tournament.ToDto();
    }

    public async Task<IEnumerable<TournamentTeamDto>> GetPublicTournamentTeamsAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null || !tournament.IsPublic)
            throw new NotFoundException("Public tournament not found");

        var tournamentTeams = await _tournamentRepository.GetTournamentTeamsAsync(tournamentId);
        // Only return approved teams for public view
        return tournamentTeams
            .Where(tt => tt.Status == "Approved")
            .Select(tt => tt.ToDto());
    }

    public async Task<IEnumerable<MatchDto>> GetPublicTournamentMatchesAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null || !tournament.IsPublic)
            throw new NotFoundException("Public tournament not found");

        var matches = await _tournamentRepository.GetTournamentMatchesAsync(tournamentId);
        return matches.Select(m => m.ToDto());
    }

    public async Task<TournamentStatisticsDto> GetPublicTournamentStatisticsAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null || !tournament.IsPublic)
            throw new NotFoundException("Public tournament not found");

        var tournamentTeams = await _tournamentRepository.GetTournamentTeamsAsync(tournamentId);
        var matches = await _tournamentRepository.GetTournamentMatchesAsync(tournamentId);
        var matchEvents = await _tournamentRepository.GetTournamentMatchEventsAsync(tournamentId);

        var approvedTeams = tournamentTeams.Where(tt => tt.Status == "Approved");
        var completedMatches = matches.Where(m => m.Status == "Completed");
        var goalEvents = matchEvents.Where(me => me.EventType == "Goal");

        // Calculate player statistics using Player entity navigation
        var playerGoals = goalEvents
            .GroupBy(ge => new { ge.PlayerId, PlayerName = ge.Player.FirstName + " " + ge.Player.LastName })
            .Select(g => new { PlayerId = g.Key.PlayerId, PlayerName = g.Key.PlayerName, Goals = g.Count() })
            .OrderByDescending(pg => pg.Goals)
            .FirstOrDefault();

        // For now, we'll use a simplified approach for assists since we don't have direct assist tracking
        // In a real system, you might track assists as separate events or as part of goal events

        return new TournamentStatisticsDto
        {
            TournamentId = tournamentId,
            TournamentName = tournament.Name,
            TotalTeams = approvedTeams.Count(),
            ApprovedTeams = approvedTeams.Count(),
            PendingTeams = tournamentTeams.Count(tt => tt.Status == "Pending"),
            TotalMatches = matches.Count(),
            CompletedMatches = completedMatches.Count(),
            ScheduledMatches = matches.Count(m => m.Status == "Scheduled"),
            TotalGoals = goalEvents.Count(),
            TotalYellowCards = matchEvents.Count(me => me.EventType == "YellowCard"),
            TotalRedCards = matchEvents.Count(me => me.EventType == "RedCard"),
            AverageGoalsPerMatch = completedMatches.Any() ? (decimal)goalEvents.Count() / completedMatches.Count() : 0,
            TopScorer = playerGoals?.PlayerName ?? "N/A",
            TopScorerGoals = playerGoals?.Goals ?? 0,
            TopAssist = "N/A", // Simplified for now
            TopAssistCount = 0, // Simplified for now
            LastUpdated = DateTime.UtcNow
        };
    }

    #endregion
}