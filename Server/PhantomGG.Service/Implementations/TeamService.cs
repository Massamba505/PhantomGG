using PhantomGG.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Common.Enums;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations;

public class TeamService(
    ITeamRepository teamRepository,
    ITournamentRepository tournamentRepository,
    ICurrentUserService currentUserService,
    IImageService imageService,
    IPlayerRepository playerRepository) : ITeamService
{
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IImageService _imageService = imageService;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    #region Guest Access - Public Team Information

    public async Task<PaginatedResponse<TeamDto>> SearchAsync(TeamSearchDto searchDto)
    {
        var teams = await _teamRepository.GetAllAsync();

        // Apply search filter
        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            teams = teams.Where(t =>
                t.Name.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (t.ShortName != null && t.ShortName.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        // Apply tournament filter
        if (searchDto.TournamentId.HasValue)
        {
            var tournamentTeams = await _tournamentRepository.GetTournamentTeamsAsync(searchDto.TournamentId.Value);
            var tournamentTeamIds = tournamentTeams.Select(t => t.Id).ToHashSet();
            teams = teams.Where(t => tournamentTeamIds.Contains(t.Id));
        }

        // Convert to DTOs
        var teamDtos = teams.Select(t => t.ToDto()).ToList();

        // Apply pagination
        var totalCount = teamDtos.Count;
        var paginatedTeams = teamDtos
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToList();

        return new PaginatedResponse<TeamDto>
        {
            Data = paginatedTeams,
            TotalRecords = totalCount,
            PageNumber = searchDto.Page,
            PageSize = searchDto.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize),
            HasNextPage = searchDto.Page < (int)Math.Ceiling((double)totalCount / searchDto.PageSize),
            HasPreviousPage = searchDto.Page > 1
        };
    }

    public async Task<TeamDto> GetByIdAsync(Guid id)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team == null)
            throw new NotFoundException("Team not found");

        return team.ToDto();
    }

    public async Task<IEnumerable<PlayerDto>> GetTeamPlayersAsync(Guid teamId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new NotFoundException("Team not found");

        var players = await _playerRepository.GetByTeamAsync(teamId);
        return players.Select(p => p.ToDto());
    }

    #endregion

    #region User Operations - Team Management

    public async Task<IEnumerable<TeamDto>> GetMyTeamsAsync(Guid userId)
    {
        var teams = await _teamRepository.GetByUserAsync(userId);
        return teams.Select(t => t.ToDto());
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid leaderId)
    {
        if (!_currentUserService.IsAuthenticated())
            throw new UnauthorizedException("You must be logged in to create a team");

        // Validate tournament exists and registration is open
        var tournament = await ValidateTournamentForRegistration(createDto.TournamentId);

        // Check if team name is unique in the tournament
        await ValidateTeamNameUniqueness(createDto.Name, createDto.TournamentId);

        // Check if tournament has space
        await ValidateTournamentCapacity(createDto.TournamentId, tournament.MaxTeams);

        var team = createDto.ToEntity(leaderId);
        var createdTeam = await _teamRepository.CreateAsync(team);

        // Register team for tournament
        await _teamRepository.RegisterForTournamentAsync(createdTeam.Id, createDto.TournamentId);

        return createdTeam.ToDto();
    }

    public async Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto updateDto, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team == null)
            throw new NotFoundException("Team not found");

        ValidateTeamOwnership(team, userId);

        if (!string.IsNullOrEmpty(updateDto.Name) && updateDto.Name != team.Name)
        {
            var tournaments = await _teamRepository.GetTournamentsByTeamAsync(team.Id);
            foreach (var tournament in tournaments)
            {
                await ValidateTeamNameUniqueness(updateDto.Name, tournament.Id);
            }
        }

        updateDto.UpdateEntity(team);
        var updatedTeam = await _teamRepository.UpdateAsync(team);
        return updatedTeam.ToDto();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team == null)
            return;

        ValidateTeamOwnership(team, userId);
        await ValidateTeamCanBeDeleted(team);

        await _teamRepository.DeleteAsync(id);
    }

    public async Task<string> UploadLogoAsync(Guid teamId, IFormFile file, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new NotFoundException("Team not found");

        ValidateTeamOwnership(team, userId);

        // Delete old logo if exists
        if (!string.IsNullOrEmpty(team.LogoUrl))
        {
            await _imageService.DeleteImageAsync(team.LogoUrl);
        }

        // Upload new logo
        var logoUrl = await _imageService.SaveImageAsync(file, ImageType.TeamLogo, teamId);

        // Update team
        team.LogoUrl = logoUrl;
        await _teamRepository.UpdateAsync(team);

        return logoUrl;
    }

    #endregion

    #region User Player Management (from Controller)

    public async Task<PlayerDto> AddPlayerToTeamAsync(Guid teamId, object playerDto, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new NotFoundException("Team not found");

        ValidateTeamOwnership(team, userId);

        if (playerDto is not CreatePlayerDto createDto)
            throw new ArgumentException("Invalid player data");

        // Check team player limit
        var currentPlayerCount = await _playerRepository.GetPlayerCountByTeamAsync(teamId);
        if (currentPlayerCount >= 11)
            throw new InvalidOperationException("Team has reached maximum player limit (11 players).");

        var player = createDto.ToEntity();
        player.TeamId = teamId;
        var createdPlayer = await _playerRepository.CreateAsync(player);

        return createdPlayer.ToDto();
    }

    public async Task<PlayerDto> UpdateTeamPlayerAsync(Guid teamId, Guid playerId, UpdatePlayerDto updateDto, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new NotFoundException("Team not found");

        ValidateTeamOwnership(team, userId);

        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new NotFoundException("Player not found");

        if (player.TeamId != teamId)
            throw new ArgumentException("Player does not belong to this team");

        updateDto.UpdateEntity(player);
        var updatedPlayer = await _playerRepository.UpdateAsync(player);

        return updatedPlayer.ToDto();
    }

    public async Task RemovePlayerFromTeamAsync(Guid teamId, Guid playerId, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new NotFoundException("Team not found");

        ValidateTeamOwnership(team, userId);

        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            return; // Player doesn't exist, nothing to remove

        if (player.TeamId != teamId)
            throw new ArgumentException("Player does not belong to this team");

        await _playerRepository.DeleteAsync(playerId);
    }

    public async Task<string> UploadPlayerPhotoAsync(Guid teamId, Guid playerId, IFormFile file, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new NotFoundException("Team not found");

        ValidateTeamOwnership(team, userId);

        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new NotFoundException("Player not found");

        if (player.TeamId != teamId)
            throw new ArgumentException("Player does not belong to this team");

        // Delete old photo if exists
        if (!string.IsNullOrEmpty(player.PhotoUrl))
        {
            await _imageService.DeleteImageAsync(player.PhotoUrl);
        }

        // Upload new photo
        var photoUrl = await _imageService.SaveImageAsync(file, ImageType.PlayerPhoto, playerId);

        // Update player
        player.PhotoUrl = photoUrl;
        await _playerRepository.UpdateAsync(player);

        return photoUrl;
    }

    #endregion

    #region Organizer Operations - Team Registration Management

    public async Task<IEnumerable<TeamDto>> GetByRegistrationStatusAsync(Guid tournamentId, string status, Guid organizerId)
    {
        await ValidateOrganizerAccess(tournamentId, organizerId);
        IEnumerable<Team> teams;
        if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            teams = await _tournamentRepository.GetPendingTeamsAsync(tournamentId);
        }
        else if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
        {
            teams = await _tournamentRepository.GetApprovedTeamsAsync(tournamentId);
        }
        else
        {
            teams = await _tournamentRepository.GetTournamentTeamsAsync(tournamentId);
        }
        return teams.Select(t => t.ToDto());
    }

    public async Task ApproveTeamAsync(Guid teamId, Guid organizerId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new NotFoundException("Team not found");

        var tournaments = await _teamRepository.GetTournamentsByTeamAsync(teamId);
        var tournament = tournaments.FirstOrDefault();
        if (tournament == null)
            throw new NotFoundException("Tournament not found for this team");

        await ValidateOrganizerAccess(tournament.Id, organizerId);
        await _teamRepository.UpdateTeamTournamentStatusAsync(tournament.Id, teamId, "Approved");
    }

    public async Task RejectTeamAsync(Guid teamId, Guid organizerId, string? reason = null)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new NotFoundException("Team not found");

        var tournaments = await _teamRepository.GetTournamentsByTeamAsync(teamId);
        var tournament = tournaments.FirstOrDefault();
        if (tournament == null)
            throw new NotFoundException("Tournament not found for this team");

        await ValidateOrganizerAccess(tournament.Id, organizerId);
        await _teamRepository.UpdateTeamTournamentStatusAsync(tournament.Id, teamId, "Rejected");
    }

    #endregion

    #region Private Helper Methods

    private async Task<Models.Entities.Tournament> ValidateTournamentForRegistration(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        if (tournament.RegistrationDeadline.HasValue && tournament.RegistrationDeadline <= DateTime.UtcNow)
            throw new InvalidOperationException("Registration deadline has passed");

        if (tournament.StartDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Cannot register for a tournament that has already started");

        return tournament;
    }

    private async Task ValidateTeamNameUniqueness(string teamName, Guid tournamentId)
    {
        var existingTeams = await _tournamentRepository.GetTournamentTeamsAsync(tournamentId);
        if (existingTeams.Any(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException("A team with this name is already registered for this tournament");
    }

    private async Task ValidateTournamentCapacity(Guid tournamentId, int maxTeams)
    {
        var currentTeamCount = await _teamRepository.GetTournamentTeamCountAsync(tournamentId);
        if (currentTeamCount >= maxTeams)
            throw new InvalidOperationException("Tournament is full");
    }

    private void ValidateTeamOwnership(Models.Entities.Team team, Guid userId)
    {
        if (team.UserId != userId)
            throw new ForbiddenException("You don't have permission to modify this team");
    }

    private async Task ValidateTeamCanBeDeleted(Models.Entities.Team team)
    {
        var tournaments = await _teamRepository.GetTournamentsByTeamAsync(team.Id);
        var activeTournaments = tournaments.Where(t => t.StartDate > DateTime.UtcNow).ToList();

        if (activeTournaments.Any())
        {
            var tournamentNames = string.Join(", ", activeTournaments.Select(t => t.Name));
            throw new InvalidOperationException($"Cannot delete team. Team is registered for upcoming tournaments: {tournamentNames}");
        }
    }

    private async Task ValidateOrganizerAccess(Guid tournamentId, Guid organizerId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        if (tournament.OrganizerId != organizerId)
            throw new ForbiddenException("You don't have permission to manage teams for this tournament");
    }

    #endregion
}
