using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations
{
    public class PlayerService(
        IPlayerRepository playerRepository,
        IImageService imageService) : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository = playerRepository;
        private readonly IImageService _imageService = imageService;

        public async Task<PlayerDto> GetByIdAsync(Guid playerId)
        {
            var player = await ValidatePlayerExistsAsync(playerId);

            return player.ToDto();
        }

        public async Task<PlayerDto> CreateAsync(CreatePlayerDto createDto)
        {
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
            var existingPlayer = await ValidatePlayerExistsAsync(playerId);

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
            var player = await ValidatePlayerExistsAsync(playerId);

            if (player.TeamId != teamId)
            {
                throw new ArgumentException("Player does not belong to this team");
            }

            if (!string.IsNullOrEmpty(player.PhotoUrl))
            {
                await _imageService.DeleteImageAsync(player.PhotoUrl);
            }

            await _playerRepository.DeleteAsync(player.Id);
        }

        public async Task<IEnumerable<PlayerDto>> GetByTeamAsync(Guid teamId)
        {
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

        private async Task<Player> ValidatePlayerExistsAsync(Guid playerId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null)
            {
                throw new NotFoundException("Player not found.");
            }

            return player;
        }
    }
}
