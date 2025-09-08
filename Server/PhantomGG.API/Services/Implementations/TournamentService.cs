using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Mappings;
using PhantomGG.API.Common;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Implementations;

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

    public async Task<IEnumerable<TournamentDto>> GetAllAsync()
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        return tournaments.Select(tournament => tournament.ToTournamentDto());
    }

    public async Task<IEnumerable<TournamentDto>> GetAllPublicAsync()
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        return tournaments.Where(t => t.IsPublic).Select(tournament => tournament.ToTournamentDto());
    }

    public async Task<TournamentDto> GetByIdAsync(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        return tournament.ToTournamentDto();
    }

    public async Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(Guid organizerId)
    {
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(organizerId);
        return tournaments.Select(tournament => tournament.ToTournamentDto());
    }

    public async Task<IEnumerable<TournamentDto>> SearchAsync(TournamentSearchDto searchDto)
    {
        var tournaments = await _tournamentRepository.SearchAsync(searchDto);
        return tournaments.Select(tournament => tournament.ToTournamentDto());
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId)
    {
        var tournament = createDto.ToTournament(organizerId);
        var createdTournament = await _tournamentRepository.CreateAsync(tournament);
        return createdTournament.ToTournamentDto();
    }

    public async Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid userId)
    {
        var existingTournament = await _tournamentRepository.GetByIdAsync(id);
        if (existingTournament == null)
            throw new ArgumentException("Tournament not found");

        if (existingTournament.OrganizerId != userId)
            throw new UnauthorizedAccessException("You don't have permission to update this tournament");

        existingTournament.UpdateFromDto(updateDto);
        var updatedTournament = await _tournamentRepository.UpdateAsync(existingTournament);
        return updatedTournament.ToTournamentDto();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (tournament.OrganizerId != userId)
            throw new UnauthorizedAccessException("You don't have permission to delete this tournament");

        await _tournamentRepository.DeleteAsync(id);
    }

    private async Task<Tournament> ChangeTournamentStatue(Guid id, Guid userId, TournamentStatus status)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (tournament.OrganizerId != userId)
            throw new UnauthorizedAccessException("You don't have permission to modify this tournament");

        tournament.Status = status.ToString();
        return tournament;
    }

    public async Task<TournamentDto> StartTournamentAsync(Guid id, Guid userId)
    {
        var tournament = await ChangeTournamentStatue(id, userId, TournamentStatus.InProgress);
        var updatedTournament = await _tournamentRepository.UpdateAsync(tournament);
        return updatedTournament.ToTournamentDto();
    }

    public async Task<TournamentDto> CompleteTournamentAsync(Guid id, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (tournament.OrganizerId != userId)
            throw new UnauthorizedAccessException("You don't have permission to modify this tournament");

        tournament = await ChangeTournamentStatue(id, userId, TournamentStatus.Completed);
        var updatedTournament = await _tournamentRepository.UpdateAsync(tournament);
        return updatedTournament.ToTournamentDto();
    }

    public async Task<TournamentDto> CancelTournamentAsync(Guid id, Guid userId)
    {
        var tournament = await ChangeTournamentStatue(id, userId, TournamentStatus.Cancelled);
        var updatedTournament = await _tournamentRepository.UpdateAsync(tournament);
        return updatedTournament.ToTournamentDto();
    }

    public async Task<TournamentDto> OpenRegistrationAsync(Guid id, Guid userId)
    {
        var tournament = await ChangeTournamentStatue(id, userId, TournamentStatus.RegistrationOpen);
        var updatedTournament = await _tournamentRepository.UpdateAsync(tournament);
        return updatedTournament.ToTournamentDto();
    }

    public async Task<TournamentDto> CloseRegistrationAsync(Guid id, Guid userId)
    {
        var tournament = await ChangeTournamentStatue(id, userId, TournamentStatus.RegistrationClosed);
        var updatedTournament = await _tournamentRepository.UpdateAsync(tournament);
        return updatedTournament.ToTournamentDto();
    }

    public async Task<bool> CanTeamRegisterAsync(Guid tournamentId, Guid teamId)
    {
        return await IsRegistrationOpenAsync(tournamentId);
    }

    public async Task<bool> IsRegistrationOpenAsync(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null) return false;

        return tournament.Status == TournamentStatus.RegistrationOpen.ToString();
    }

    public async Task<int> GetRegisteredTeamCountAsync(Guid tournamentId)
    {
        return await _tournamentRepository.GetTeamCountAsync(tournamentId);
    }

    public async Task<int> GetApprovedTeamCountAsync(Guid tournamentId)
    {
        return await _tournamentRepository.GetTeamCountAsync(tournamentId);
    }

    public async Task JoinTournamentAsync(Guid tournamentId, Guid teamId, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (!await IsRegistrationOpenAsync(tournamentId))
            throw new InvalidOperationException("Registration is not open for this tournament");

        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new ArgumentException("Team not found");

        var currentUser = _currentUserService.GetCurrentUser();
        if (team.ManagerEmail != currentUser.Email)
            throw new UnauthorizedAccessException("You don't have permission to register this team");

        if (team.TournamentId == tournamentId)
            throw new InvalidOperationException("Team is already registered for this tournament");

        if (team.TournamentId != Guid.Empty && team.TournamentId != tournamentId)
            throw new InvalidOperationException("Team is already registered for another tournament");

        var currentTeamCount = await GetRegisteredTeamCountAsync(tournamentId);
        if (currentTeamCount >= tournament.MaxTeams)
            throw new InvalidOperationException("Tournament is full");

        team.TournamentId = tournamentId;
        team.RegistrationStatus = TeamRegistrationStatus.Pending.ToString();
        team.RegistrationDate = DateTime.UtcNow;

        await _teamRepository.UpdateAsync(team);
    }

    public async Task LeaveTournamentAsync(Guid tournamentId, Guid teamId, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new ArgumentException("Team not found");

        var currentUser = _currentUserService.GetCurrentUser();
        if (team.ManagerEmail != currentUser.Email)
            throw new UnauthorizedAccessException("You don't have permission to unregister this team");

        if (team.TournamentId != tournamentId)
            throw new InvalidOperationException("Team is not registered for this tournament");

        if (tournament.Status == TournamentStatus.InProgress.ToString())
            throw new InvalidOperationException("Cannot leave tournament after it has started");

        team.TournamentId = Guid.Empty;
        team.RegistrationStatus = TeamRegistrationStatus.Withdrawn.ToString();
        team.ApprovedDate = null;

        await _teamRepository.UpdateAsync(team);
    }

    public async Task<string> UploadTournamentBannerAsync(Guid tournamentId, IFormFile file, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (tournament.OrganizerId != userId)
            throw new UnauthorizedAccessException("You don't have permission to upload banner for this tournament");

        if (!string.IsNullOrEmpty(tournament.BannerUrl))
        {
            await _imageService.DeleteImageAsync(tournament.BannerUrl);
        }

        var bannerUrl = await _imageService.SaveImageAsync(file, ImageType.TournamentBanner, tournamentId);
        tournament.BannerUrl = bannerUrl;
        await _tournamentRepository.UpdateAsync(tournament);

        return bannerUrl;
    }

    public async Task<string> UploadTournamentLogoAsync(Guid tournamentId, IFormFile file, Guid userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new ArgumentException("Tournament not found");

        if (tournament.OrganizerId != userId)
            throw new UnauthorizedAccessException("You don't have permission to upload logo for this tournament");

        // Delete old logo if exists
        if (!string.IsNullOrEmpty(tournament.LogoUrl))
        {
            await _imageService.DeleteImageAsync(tournament.LogoUrl);
        }

        // Upload new logo
        var logoUrl = await _imageService.SaveImageAsync(file, ImageType.TournamentLogo, tournamentId);

        // Update tournament
        tournament.LogoUrl = logoUrl;
        await _tournamentRepository.UpdateAsync(tournament);

        return logoUrl;
    }

    public async Task<bool> IsOrganizerAsync(Guid tournamentId, Guid userId)
    {
        return await _tournamentRepository.IsOrganizerAsync(tournamentId, userId);
    }

    public async Task<bool> CanUserManageTournamentAsync(Guid tournamentId, Guid userId)
    {
        return await IsOrganizerAsync(tournamentId, userId);
    }
}
