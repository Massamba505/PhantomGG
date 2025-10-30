using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Domain.Teams.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Mappings;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Domain.Teams.Implementations;

public class PlayerService(
    IPlayerRepository playerRepository,
    IImageService imageService,
    IPlayerValidationService playerValidationService,
    ITeamValidationService teamValidationService,
    ITeamRepository teamRepository) : IPlayerService
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IPlayerValidationService _playerValidationService = playerValidationService;
    private readonly ITeamValidationService _teamValidationService = teamValidationService;
    private readonly ITeamRepository _teamRepository = teamRepository;

    public async Task<PlayerDto> GetByIdAsync(Guid playerId)
    {
        var player = await _playerValidationService.ValidatePlayerExistsAsync(playerId);

        return player.ToDto();
    }

    public async Task<PlayerDto> CreateAsync(CreatePlayerDto createDto)
    {
        await _teamValidationService.ValidateTeamExistsAsync(createDto.TeamId);
        await _playerValidationService.ValidateMaxPlayersPerTeamAsync(createDto.TeamId, 15);
        await _playerValidationService.ValidatePlayerPositionDistributionAsync(createDto.TeamId, (int)createDto.Position);

        if (!string.IsNullOrEmpty(createDto.Email))
        {
            await _playerValidationService.ValidateEmailUniquenessWithinTeamAsync(createDto.Email, createDto.TeamId);
        }

        var player = createDto.ToEntity();
        if (createDto.PhotoUrl != null)
        {
            player.PhotoUrl = await UploadPlayerPhotoAsync(player, createDto.PhotoUrl);
        }

        var createdPlayer = await _playerRepository.CreateAsync(player);
        return createdPlayer.ToDto();
    }

    public async Task<PlayerDto> UpdateAsync(UpdatePlayerDto updateDto, Guid playerId)
    {
        var existingPlayer = await _playerValidationService.ValidatePlayerExistsAsync(playerId);
        if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != existingPlayer.Email)
        {
            await _playerValidationService.ValidateEmailUniquenessWithinTeamAsync(updateDto.Email, existingPlayer.TeamId, playerId);
        }

        if (updateDto.Position.HasValue && (int)updateDto.Position.Value != existingPlayer.Position)
        {
            await _playerValidationService.ValidatePlayerPositionDistributionAsync(existingPlayer.TeamId, (int)updateDto.Position.Value);
        }

        updateDto.UpdateEntity(existingPlayer);

        if (updateDto.PhotoUrl != null)
        {
            existingPlayer.PhotoUrl = await UploadPlayerPhotoAsync(existingPlayer, updateDto.PhotoUrl);
        }

        var updatedPlayer = await _playerRepository.UpdateAsync(existingPlayer);

        return updatedPlayer.ToDto();
    }

    public async Task DeleteAsync(Guid teamId, Guid playerId)
    {
        var player = await _playerValidationService.ValidatePlayerExistsAsync(playerId);

        await _playerValidationService.ValidatePlayerBelongsToTeamAsync(playerId, teamId);
        await _playerValidationService.ValidatePlayerNotInMatchAsync(playerId);
        await _playerValidationService.ValidateMinPlayersPerTeamAsync(teamId, 1);

        if (!string.IsNullOrEmpty(player.PhotoUrl))
        {
            await _imageService.DeleteImageAsync(player.PhotoUrl);
        }

        await _playerRepository.DeleteAsync(player.Id);
    }

    public async Task<IEnumerable<PlayerDto>> GetByTeamAsync(Guid teamId)
    {
        await _teamValidationService.ValidateTeamExistsAsync(teamId);
        var players = await _playerRepository.GetByTeamAsync(teamId);

        return players.Select(p => p.ToDto());
    }

    private async Task<string> UploadPlayerPhotoAsync(Player player, IFormFile photo)
    {
        if (!string.IsNullOrEmpty(player.PhotoUrl))
        {
            await _imageService.DeleteImageAsync(player.PhotoUrl);
        }

        var photoUrl = await _imageService.SaveImageAsync(photo, ImageType.PlayerPhoto, player.Id);

        return photoUrl;
    }
}
