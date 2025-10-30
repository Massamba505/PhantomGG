using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Image;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Specifications;
using PhantomGG.Service.Domain.Teams.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Mappings;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Domain.Teams.Implementations;

public class TeamService(
    ITeamRepository teamRepository,
    ITournamentTeamRepository tournamentTeamRepository,
    IImageService imageService,
    IPlayerService playerService,
    ITeamValidationService teamValidationService,
    ITournamentValidationService tournamentValidationService,
    IPlayerRepository playerRepository,
    ICacheInvalidationService cacheInvalidationService,
    HybridCache cache) : ITeamService
{
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IPlayerService _playerService = playerService;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly ITeamValidationService _teamValidationService = teamValidationService;
    private readonly ITournamentValidationService _tournamentValidationService = tournamentValidationService;
    private readonly HybridCache _cache = cache;
    private readonly ICacheInvalidationService _cacheInvalidationService = cacheInvalidationService;

    public async Task<PagedResult<TeamDto>> SearchAsync(TeamQuery query, Guid? userId = null)
    {
        if (query.TournamentId.HasValue)
        {
            await _tournamentValidationService.ValidateTournamentExistsAsync(query.TournamentId.Value);
        }

        var spec = new TeamSpecification
        {
            SearchTerm = query.Q,
            TournamentId = query.TournamentId,
            Status = query.Status,
            UserId = userId,
            Page = query.Page,
            PageSize = query.PageSize
        };

        var teams = await _teamRepository.SearchAsync(spec);
        return new PagedResult<TeamDto>(
            teams.Data.Select(t => t.ToDto()),
            query.Page,
            query.PageSize,
            teams.Meta.TotalRecords
        );
    }

    public async Task<TeamDto> GetByIdAsync(Guid teamId)
    {
        return await _cache.GetOrCreateAsync(
            $"team_{teamId}",
            async cancel =>
            {
                var team = await _teamValidationService.ValidateTeamExistsAsync(teamId);
                return team.ToDto();
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(20)
            },
            cancellationToken: CancellationToken.None
        );
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid managerId)
    {
        await ValidateMaxTeamsPerUserAsync(managerId);
        await _teamValidationService.ValidateUserTeamNameUniqueness(createDto.Name, managerId);
        var team = createDto.ToEntity(managerId);

        await _teamRepository.CreateAsync(team);

        if (createDto.LogoUrl != null)
        {
            var uploadImage = new UploadImageRequest
            {
                OldFileUrl = team.LogoUrl,
                File = createDto.LogoUrl,
                ImageType = ImageType.TournamentBanner,
                Id = team.Id
            };

            team.LogoUrl = await _imageService.UploadImageAsync(uploadImage);
        }

        await _teamRepository.UpdateAsync(team);
        await _cacheInvalidationService.InvalidateTeamRelatedCacheAsync(team.Id);

        return team.ToDto();
    }

    public async Task<TeamDto> UpdateAsync(Guid teamId, UpdateTeamDto updateDto, Guid userId)
    {
        var existingTeam = await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);

        if (!string.IsNullOrEmpty(updateDto.Name) && updateDto.Name != existingTeam.Name)
        {
            await _teamValidationService.ValidateUserTeamNameUniqueness(updateDto.Name, userId);
            await ValidateTeamNameInActiveTournamentsAsync(existingTeam, updateDto.Name);
        }

        updateDto.UpdateEntity(existingTeam);
        if (updateDto.LogoUrl != null)
        {
            var uploadImage = new UploadImageRequest
            {
                OldFileUrl = existingTeam.LogoUrl,
                File = updateDto.LogoUrl,
                ImageType = ImageType.TournamentBanner,
                Id = existingTeam.Id
            };
            existingTeam.LogoUrl = await _imageService.UploadImageAsync(uploadImage);
        }

        var updatedTeam = await _teamRepository.UpdateAsync(existingTeam);
        await _cacheInvalidationService.InvalidateTeamRelatedCacheAsync(updatedTeam.Id);
        return updatedTeam.ToDto();
    }

    public async Task DeleteAsync(Guid teamId, Guid userId)
    {
        var team = await _teamValidationService.ValidateTeamCanBeDeleted(teamId, userId);
        await _teamRepository.DeleteAsync(team.Id);
        await _cacheInvalidationService.InvalidateTeamRelatedCacheAsync(team.Id);
    }

    public async Task<IEnumerable<PlayerDto>> GetTeamPlayersAsync(Guid teamId)
    {
        return await _cache.GetOrCreateAsync(
            $"team_players_{teamId}",
            async cancel =>
            {
                var team = await _teamValidationService.ValidateTeamExistsAsync(teamId);
                var players = await _playerRepository.GetByTeamAsync(teamId);
                return players.Select(p => p.ToDto());
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(25)
            },
            cancellationToken: CancellationToken.None
        );
    }

    public async Task<PlayerDto> AddPlayerToTeamAsync(Guid teamId, CreatePlayerDto playerDto, Guid userId)
    {
        var team = await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);

        var currentPlayerCount = await _playerRepository.GetPlayerCountByTeamAsync(team.Id);
        if (currentPlayerCount >= 15)
        {
            throw new ValidationException("Team already has the maximum number of players (15)");
        }

        if (teamId != playerDto.TeamId)
        {
            throw new ValidationException("Player's TeamId does not match the specified team Id");
        }

        var player = await _playerService.CreateAsync(playerDto);
        await _cacheInvalidationService.InvalidateTeamCacheAsync(teamId);
        return player;
    }

    public async Task<PlayerDto> UpdateTeamPlayerAsync(Guid teamId, Guid playerId, UpdatePlayerDto updateDto, Guid userId)
    {
        var team = await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);

        var updatedPlayer = await _playerService.UpdateAsync(updateDto, playerId);
        await _cacheInvalidationService.InvalidateTeamCacheAsync(team.Id);
        return updatedPlayer;
    }

    public async Task RemovePlayerFromTeamAsync(Guid teamId, Guid playerId, Guid userId)
    {
        var team = await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);
        await _playerService.DeleteAsync(team.Id, playerId);
        await _cacheInvalidationService.InvalidateTeamCacheAsync(team.Id);
    }

    private async Task ValidateMaxTeamsPerUserAsync(Guid managerId)
    {
        var existingTeams = await _teamRepository.GetByUserAsync(managerId);
        if (existingTeams.Count() >= 5)
        {
            throw new ForbiddenException("You cannot create more than 5 teams. Please delete an existing team first.");
        }
    }

    private async Task ValidateTeamNameInActiveTournamentsAsync(PhantomGG.Repository.Entities.Team existingTeam, string newName)
    {
        var tournaments = await _tournamentTeamRepository.GetTournamentsByTeamAsync(existingTeam.Id);
        var activeTournaments = tournaments.Where(t => t.Status != (int)TournamentStatus.Completed);

        foreach (var tournament in activeTournaments)
        {
            var existingTeams = await _tournamentTeamRepository.GetByTournamentAsync(tournament.Id);
            if (existingTeams.Any(tt => tt.Team.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException($"A team with this name is already registered in the tournament '{tournament.Name}'");
            }
        }
    }
}