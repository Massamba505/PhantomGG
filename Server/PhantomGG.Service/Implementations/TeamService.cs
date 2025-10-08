using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Image;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Specifications;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations;

public class TeamService(
    ITeamRepository teamRepository,
    ITournamentTeamRepository tournamentTeamRepository,
    IImageService imageService,
    IPlayerService playerService,
    ITeamValidationService teamValidationService,
    IPlayerRepository playerRepository,
    HybridCache cache) : ITeamService
{
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IPlayerService _playerService = playerService;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly ITeamValidationService _teamValidationService = teamValidationService;
    private readonly HybridCache _cache = cache;

    public async Task<PagedResult<TeamDto>> SearchAsync(TeamQuery query, Guid? userId = null)
    {
        var spec = new TeamSpecification
        {
            SearchTerm = query.Q,
            TournamentId = query.TournamentId,
            Status = query.Status,
            UserId = userId,
            Page = query.Page,
            PageSize = query.PageSize
        };

        string cacheKey = spec.GetDeterministicKey();

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel =>
            {
                var teams = await _teamRepository.SearchAsync(spec);
                return new PagedResult<TeamDto>(
                    teams.Data.Select(t => t.ToDto()),
                    query.Page,
                    query.PageSize,
                    teams.Meta.TotalRecords
                );
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5)
            },
            cancellationToken: CancellationToken.None
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

        return team.ToDto();
    }

    public async Task<TeamDto> UpdateAsync(Guid teamId, UpdateTeamDto updateDto, Guid userId)
    {
        var existingTeam = await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);

        if (!string.IsNullOrEmpty(updateDto.Name) && updateDto.Name != existingTeam.Name)
        {
            await _teamValidationService.ValidateUserTeamNameUniqueness(updateDto.Name, userId);

            var tournaments = await _tournamentTeamRepository.GetTournamentsByTeamAsync(existingTeam.Id);
            tournaments = tournaments.Where(t => t.Status != TournamentStatus.Completed.ToString());
            foreach (var tournament in tournaments)
            {
                var existingTeams = await _tournamentTeamRepository.GetByTournamentAsync(tournament.Id);
                if (existingTeams.Any(tt => tt.Team.Name.Equals(updateDto.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ConflictException($"A team with this name is already registered in the tournament '{tournament.Name}'");
                }
            }
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
        return updatedTeam.ToDto();
    }

    public async Task DeleteAsync(Guid teamId, Guid userId)
    {
        var team = await _teamValidationService.ValidateTeamCanBeDeleted(teamId, userId);
        await _teamRepository.DeleteAsync(team.Id);
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
        if (currentPlayerCount >= 11)
        {
            throw new ValidationException("Team already has the maximum number of players (11)");
        }

        if (teamId != playerDto.TeamId)
        {
            throw new ValidationException("Player's TeamId does not match the specified team Id");
        }

        var player = await _playerService.CreateAsync(playerDto);
        return player;
    }

    public async Task<PlayerDto> UpdateTeamPlayerAsync(Guid teamId, Guid playerId, UpdatePlayerDto updateDto, Guid userId)
    {
        var team = await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);

        var updatedPlayer = await _playerService.UpdateAsync(updateDto, playerId);
        return updatedPlayer;
    }

    public async Task RemovePlayerFromTeamAsync(Guid teamId, Guid playerId, Guid userId)
    {
        var team = await _teamValidationService.ValidateCanManageTeamAsync(userId, teamId);
        await _playerService.DeleteAsync(team.Id, playerId);
    }
}