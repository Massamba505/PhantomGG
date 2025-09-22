using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;
using System.Numerics;

namespace PhantomGG.Service.Implementations;

public class TeamService(
    ITeamRepository teamRepository,
    ITournamentRepository tournamentRepository,
    IImageService imageService,
    IPlayerService playerService,
    IPlayerRepository playerRepository) : ITeamService
{
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IPlayerService _playerService = playerService;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public async Task<PaginatedResponse<TeamDto>> SearchAsync(TeamSearchDto searchDto)
    {
        var teams = await _teamRepository.SearchAsync(searchDto);
        var teamDtos = teams.Select(t => t.ToDto());
        var totalCount = teamDtos.Count();

        return new PaginatedResponse<TeamDto>
        {
            Data = teamDtos,
            TotalRecords = totalCount,
            PageNumber = searchDto.Page,
            PageSize = searchDto.PageSize
        };
    }

    public async Task<TeamDto> GetByIdAsync(Guid teamId)
    {
        var team = await ValidateTeamExistsAsync(teamId);

        return team.ToDto();
    }

    public async Task<IEnumerable<PlayerDto>> GetTeamPlayersAsync(Guid teamId)
    {
        var team = await ValidateTeamExistsAsync(teamId);

        var players = await _playerRepository.GetByTeamAsync(teamId);

        return players.Select(p => p.ToDto());
    }

    public async Task<IEnumerable<TeamDto>> GetMyTeamsAsync(Guid userId)
    {
        var teams = await _teamRepository.GetByUserAsync(userId);
        return teams.Select(t => t.ToDto());
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid managerId)
    {
        await ValidateUserTeamNameUniqueness(createDto.Name, managerId);
        var team = createDto.ToEntity(managerId);

        if (createDto.LogoUrl != null)
        {
            var logoUrl = await UploadLogoAsync(team, createDto.LogoUrl);
            team.LogoUrl = logoUrl;
        }
        else
        {
            team.LogoUrl = $"https://placehold.co/200x200?text={team.Name}"
        }

            var createdTeam = await _teamRepository.CreateAsync(team);

        return createdTeam.ToDto();
    }

    public async Task<TeamDto> UpdateAsync(Guid teamId, UpdateTeamDto updateDto, Guid userId)
    {
        var existingTeam = await ValidateTeamExistsAsync(teamId);

        ValidateTeamOwnership(existingTeam, userId);

        if (!string.IsNullOrEmpty(updateDto.Name) && updateDto.Name != existingTeam.Name)
        {
            await ValidateUserTeamNameUniqueness(updateDto.Name, userId);

            var tournaments = await _tournamentRepository.GetTournamentsByTeamAsync(existingTeam.Id);
            tournaments = tournaments.Where(t => t.Status != TournamentStatus.Completed.ToString());
            foreach (var tournament in tournaments)
            {
                var existingTeams = await _tournamentRepository.GetTournamentTeamsAsync(tournament.Id);
                if (existingTeams.Any(tt => tt.Team.Name.Equals(updateDto.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ConflictException($"A team with this name is already registered in the tournament '{tournament.Name}'");
                }
            }
        }

        updateDto.UpdateEntity(existingTeam);
        if (updateDto.LogoUrl != null)
        {
            existingTeam.LogoUrl = await UploadLogoAsync(existingTeam, updateDto.LogoUrl);
        }

        var updatedTeam = await _teamRepository.UpdateAsync(existingTeam);
        return updatedTeam.ToDto();
    }

    public async Task DeleteAsync(Guid teamId, Guid userId)
    {
        var team = await ValidateTeamExistsAsync(teamId);

        ValidateTeamOwnership(team, userId);
        await ValidateTeamCanBeDeleted(team);

        await _teamRepository.DeleteAsync(teamId);
    }

    public async Task<string> UploadLogoAsync(Team team, IFormFile file)
    {
        if (!string.IsNullOrEmpty(team.LogoUrl))
        {
            await _imageService.DeleteImageAsync(team.LogoUrl);
        }

        var logoUrl = await _imageService.SaveImageAsync(file, ImageType.TeamLogo, team.Id);

        return logoUrl;
    }

    public async Task<PlayerDto> AddPlayerToTeamAsync(Guid teamId, CreatePlayerDto playerDto, Guid userId)
    {
        var team = await ValidateTeamExistsAsync(teamId);

        ValidateTeamOwnership(team, userId);

        var currentPlayerCount = await _playerRepository.GetPlayerCountByTeamAsync(teamId);
        if (currentPlayerCount >= 11) { 
            throw new ValidationException("Team already has the maximum number of players (11)");
        }

        if (teamId != playerDto.TeamId) { 
            throw new ValidationException("Player's TeamId does not match the specified team Id");
        }

        var player = await _playerService.CreateAsync(playerDto);

        return player;
    }

    public async Task<PlayerDto> UpdateTeamPlayerAsync(Guid teamId, Guid playerId, UpdatePlayerDto updateDto, Guid userId)
    {
        var team = await ValidateTeamExistsAsync(teamId);

        ValidateTeamOwnership(team, userId);

        var updatedPlayer = await _playerService.UpdateAsync(updateDto, playerId);

        return updatedPlayer;
    }

    public async Task RemovePlayerFromTeamAsync(Guid teamId, Guid playerId, Guid userId)
    {
        var team = await ValidateTeamExistsAsync(teamId);

        ValidateTeamOwnership(team, userId);

        await _playerService.DeleteAsync(teamId, playerId);
    }

    private async Task ValidateUserTeamNameUniqueness(string teamName, Guid managerId)
    {
        var existingTeams = await _teamRepository.GetByUserAsync(managerId);
        if (existingTeams.Any(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException("A team with this name is already available");
        }
    }

    private void ValidateTeamOwnership(Team team, Guid userId)
    {
        if (team.UserId != userId)
        {
            throw new ForbiddenException("You don't have permission to modify this team");
        }
    }

    private async Task ValidateTeamCanBeDeleted(Team team)
    {
        var tournaments = await _tournamentRepository.GetTournamentsByTeamAsync(team.Id);
        var activeTournaments = tournaments.Where(t => t.Status != TournamentStatus.Completed.ToString()).ToList();

        if (activeTournaments.Any())
        {
            var tournamentNames = string.Join(", ", activeTournaments.Select(t => t.Name));
            throw new InvalidOperationException($"Cannot delete team. Team is registered in tournaments: {tournamentNames}");
        }
    }

    private async Task<Team> ValidateTeamExistsAsync(Guid teamId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
        {
            throw new NotFoundException("Team not found.");
        }

        return team;
    }
}
