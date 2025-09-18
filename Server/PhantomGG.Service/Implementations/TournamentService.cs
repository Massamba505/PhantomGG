using PhantomGG.Service.Interfaces;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.TournamentStanding;
using PhantomGG.Service.Exceptions;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Models.Entities;
using PhantomGG.Service.Mappings;
using Microsoft.AspNetCore.Http;

namespace PhantomGG.Service.Implementations;

public class TournamentService(
    ITournamentRepository tournamentRepository,
    ITeamRepository teamRepository,
    ICurrentUserService currentUserService,
    IImageService imageService) : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IImageService _imageService = imageService;

    #region Public Tournament Operations (Guest Access)

    public async Task<IEnumerable<TournamentDto>> GetPublicTournamentsAsync()
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        return tournaments.Where(t => t.IsPublic)
                         .Select(t => t.ToDto())
                         .OrderByDescending(t => t.CreatedAt);
    }

    public async Task<TournamentDto> GetByIdAsync(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        return tournament.ToDto();
    }

    public async Task<PaginatedResponse<TournamentDto>> SearchAsync(TournamentSearchDto searchDto)
    {
        var paginatedTournaments = await _tournamentRepository.SearchAsync(searchDto);

        return new PaginatedResponse<TournamentDto>(
            paginatedTournaments.Data.Select(t => t.ToDto()),
            paginatedTournaments.PageNumber,
            paginatedTournaments.PageSize,
            paginatedTournaments.TotalRecords
        );
    }

    public async Task<IEnumerable<TeamDto>> GetTournamentTeamsAsync(Guid tournamentId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);
        var teams = await _tournamentRepository.GetTournamentTeamsAsync(tournamentId);
        return teams.Select(t => t.ToDto());
    }

    public async Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId)
    {
        await ValidateTournamentExistsAsync(tournamentId);
        // For MVP, return empty standings - this would be calculated based on matches
        return new List<TournamentStandingDto>();
    }

    public async Task<IEnumerable<MatchDto>> GetTournamentMatchesAsync(Guid tournamentId)
    {
        await ValidateTournamentExistsAsync(tournamentId);
        // For MVP, return empty matches - this would come from Match repository
        return new List<MatchDto>();
    }

    #endregion

    #region User Operations (from Controller)

    public async Task RegisterForTournamentAsync(Guid tournamentId, object registrationDto, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (registrationDto is not JoinTournamentDto joinDto)
            throw new ArgumentException("Invalid registration data");

        // Validate tournament is open for registration
        if (tournament.RegistrationDeadline.HasValue && DateTime.UtcNow > tournament.RegistrationDeadline.Value)
            throw new InvalidOperationException("Registration deadline has passed");

        // Validate team exists and user owns it
        var team = await _teamRepository.GetByIdAsync(joinDto.TeamId);
        if (team == null)
            throw new ArgumentException("Team not found");

        if (team.UserId != userId)
            throw new UnauthorizedException("You don't have permission to register this team");

        // Check if team is already registered
        if (await _tournamentRepository.IsTeamRegisteredAsync(tournamentId, joinDto.TeamId))
            throw new InvalidOperationException("Team is already registered for this tournament");

        // Check tournament capacity
        var approvedCount = await _tournamentRepository.GetApprovedTeamCountAsync(tournamentId);
        if (approvedCount >= tournament.MaxTeams)
            throw new InvalidOperationException("Tournament is full");

        // Register team for tournament (status will be "Pending" or "Approved" based on tournament settings)
        await _teamRepository.RegisterForTournamentAsync(joinDto.TeamId, tournamentId);
    }

    public async Task WithdrawFromTournamentAsync(Guid tournamentId, object withdrawDto, Guid userId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);

        if (withdrawDto is not LeaveTournamentDto leaveDto)
            throw new ArgumentException("Invalid withdrawal data");

        // Validate team exists and user owns it
        var team = await _teamRepository.GetByIdAsync(leaveDto.TeamId);
        if (team == null)
            throw new ArgumentException("Team not found");

        if (team.UserId != userId)
            throw new UnauthorizedException("You don't have permission to withdraw this team");

        // Check if team is registered
        if (!await _tournamentRepository.IsTeamRegisteredAsync(tournamentId, leaveDto.TeamId))
            throw new InvalidOperationException("Team is not registered for this tournament");

        // Don't allow withdrawal if tournament has started or matches exist
        if (DateTime.UtcNow >= tournament.StartDate)
            throw new InvalidOperationException("Cannot withdraw from tournament that has already started");

        // Remove team from tournament
        await _teamRepository.RemoveFromTournamentAsync(tournamentId, leaveDto.TeamId);
    }

    #endregion

    #region Organizer Tournament Management

    public async Task<PaginatedResponse<TournamentDto>> GetMyTournamentsAsync(TournamentSearchDto searchDto, Guid organizerId)
    {
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(organizerId);
        var filteredTournaments = tournaments.OrderByDescending(t => t.CreatedAt);

        var totalCount = filteredTournaments.Count();
        var pagedTournaments = filteredTournaments
            .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .Select(t => t.ToDto())
            .ToList();

        return new PaginatedResponse<TournamentDto>
        {
            Data = pagedTournaments,
            TotalRecords = totalCount,
            PageNumber = searchDto.PageNumber,
            PageSize = searchDto.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize),
            HasNextPage = searchDto.PageNumber < (int)Math.Ceiling(totalCount / (double)searchDto.PageSize),
            HasPreviousPage = searchDto.PageNumber > 1
        };
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId)
    {
        ValidateCreateTournament(createDto);

        var tournament = createDto.ToEntity(organizerId);
        await _tournamentRepository.CreateAsync(tournament);
        return tournament.ToDto();
    }

    public async Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid organizerId)
    {
        var tournament = await ValidateOrganizerAccessAsync(id, organizerId);

        updateDto.UpdateEntity(tournament);
        await _tournamentRepository.UpdateAsync(tournament);
        return tournament.ToDto();
    }

    public async Task DeleteAsync(Guid id, Guid organizerId)
    {
        var tournament = await ValidateOrganizerAccessAsync(id, organizerId);

        // Don't allow deletion if teams are registered
        var teamCount = await _teamRepository.GetTournamentTeamCountAsync(id);
        if (teamCount > 0)
            throw new InvalidOperationException("Cannot delete tournament with registered teams");

        await _tournamentRepository.DeleteAsync(id);
    }

    public async Task<string> UploadImageAsync(Guid tournamentId, IFormFile file, Guid organizerId)
    {
        var tournament = await ValidateOrganizerAccessAsync(tournamentId, organizerId);

        // Delete old banner if exists
        if (!string.IsNullOrEmpty(tournament.BannerUrl))
        {
            await _imageService.DeleteImageAsync(tournament.BannerUrl);
        }

        // Upload new banner image
        var bannerUrl = await _imageService.SaveImageAsync(file, Common.Enums.ImageType.TournamentBanner, tournamentId);

        // Update tournament
        tournament.BannerUrl = bannerUrl;
        await _tournamentRepository.UpdateAsync(tournament);

        return bannerUrl;
    }

    #endregion

    #region Team Registration Management

    public async Task<IEnumerable<TeamDto>> GetPendingTeamsAsync(Guid tournamentId, Guid organizerId)
    {
        await ValidateOrganizerAccessAsync(tournamentId, organizerId);
        var teams = await _tournamentRepository.GetPendingTeamsAsync(tournamentId);
        return teams.Select(t => t.ToDto());
    }

    public async Task ApproveTeamAsync(Guid tournamentId, Guid teamId, Guid organizerId)
    {
        await ValidateOrganizerAccessAsync(tournamentId, organizerId);
        await _teamRepository.UpdateTeamTournamentStatusAsync(tournamentId, teamId, "Approved");
    }

    public async Task RejectTeamAsync(Guid tournamentId, Guid teamId, PhantomGG.Models.DTOs.Team.RejectTeamDto rejectDto, Guid organizerId)
    {
        await ValidateOrganizerAccessAsync(tournamentId, organizerId);
        await _teamRepository.UpdateTeamTournamentStatusAsync(tournamentId, teamId, "Rejected");
    }

    public async Task RemoveTeamAsync(Guid tournamentId, Guid teamId, Guid organizerId)
    {
        await ValidateOrganizerAccessAsync(tournamentId, organizerId);
        await _teamRepository.RemoveFromTournamentAsync(tournamentId, teamId);
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

    #endregion

    #region Private Helper Methods

    private async Task<Tournament> ValidateTournamentExistsAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");
        return tournament;
    }

    private async Task<Tournament> ValidateOrganizerAccessAsync(Guid tournamentId, Guid organizerId)
    {
        var tournament = await ValidateTournamentExistsAsync(tournamentId);
        if (tournament.OrganizerId != organizerId)
            throw new UnauthorizedException("You don't have permission to access this tournament");
        return tournament;
    }

    private static void ValidateCreateTournament(CreateTournamentDto createDto)
    {
        if (createDto.StartDate <= DateTime.UtcNow)
            throw new ArgumentException("Tournament start date must be in the future");

        if (createDto.EndDate <= createDto.StartDate)
            throw new ArgumentException("Tournament end date must be after start date");

        if (createDto.RegistrationDeadline.HasValue && createDto.RegistrationDeadline >= createDto.StartDate)
            throw new ArgumentException("Registration deadline must be before tournament start date");
    }

    #endregion
}
