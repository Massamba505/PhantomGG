using PhantomGG.Service.Mappings;
using PhantomGG.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Common.Enums;

namespace PhantomGG.Service.Implementations
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;

        public PlayerService(
            IPlayerRepository playerRepository,
            ITeamRepository teamRepository,
            ITournamentRepository tournamentRepository,
            ICurrentUserService currentUserService,
            IImageService imageService)
        {
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _tournamentRepository = tournamentRepository;
            _currentUserService = currentUserService;
            _imageService = imageService;
        }

        public async Task<PlayerDto> GetByIdAsync(Guid id)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                throw new ArgumentException("Player not found.");

            return player.ToDto();
        }

        public async Task<PlayerDto> CreateAsync(CreatePlayerDto createDto, Guid teamId, Guid userId)
        {
            // Validate user is authenticated
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedException("You must be logged in to create a player.");

            // Validate team exists
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            // Check permissions - only team owner can add players
            if (team.UserId != userId)
                throw new UnauthorizedException("You don't have permission to add players to this team.");

            // Check team player limit (assuming max 11 players per team for now)
            var currentPlayerCount = await _playerRepository.GetPlayerCountByTeamAsync(teamId);
            if (currentPlayerCount >= 11)
                throw new InvalidOperationException("Team has reached maximum player limit (11 players).");

            var player = createDto.ToEntity();
            player.TeamId = teamId; // Ensure the player is associated with the correct team
            var createdPlayer = await _playerRepository.CreateAsync(player);

            return createdPlayer.ToDto();
        }

        public async Task<PlayerDto> UpdateAsync(Guid id, UpdatePlayerDto updateDto, Guid userId)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
                throw new ArgumentException("Player not found.");

            // Check permissions
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedException("You must be logged in to update a player.");

            var team = await _teamRepository.GetByIdAsync(existingPlayer.TeamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            // Only team owner can update players
            if (team.UserId != userId)
                throw new UnauthorizedException("You don't have permission to update this player.");

            updateDto.UpdateEntity(existingPlayer);
            var updatedPlayer = await _playerRepository.UpdateAsync(existingPlayer);

            return updatedPlayer.ToDto();
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                return;

            // Check permissions
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedException("You must be logged in to delete a player.");

            var team = await _teamRepository.GetByIdAsync(player.TeamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            // Only team owner can delete players
            if (team.UserId != userId)
                throw new UnauthorizedException("You don't have permission to delete this player.");

            await _playerRepository.DeleteAsync(id);
        }

        public async Task<bool> IsPlayerOwnedByUserAsync(Guid playerId, Guid userId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null) return false;

            if (!_currentUserService.IsAuthenticated()) return false;

            var team = await _teamRepository.GetByIdAsync(player.TeamId);
            if (team == null) return false;

            // Check if the current user owns the team that the player belongs to
            return team.UserId == userId;
        }

        public async Task<string> UploadPlayerPhotoAsync(Guid playerId, IFormFile file, Guid userId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null)
                throw new ArgumentException("Player not found.");

            // Check permissions
            if (!await IsPlayerOwnedByUserAsync(playerId, userId))
                throw new UnauthorizedException("You don't have permission to update this player's photo.");

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

        public async Task<IEnumerable<PlayerDto>> GetTopScorersAsync(Guid tournamentId, int limit = 10)
        {
            // For MVP, return empty list - this would require match statistics
            // TODO: Implement when match events are tracked
            return await Task.FromResult(new List<PlayerDto>());
        }

        public async Task<IEnumerable<PlayerDto>> GetTopAssistsAsync(Guid tournamentId, int limit = 10)
        {
            // For MVP, return empty list - this would require match statistics
            // TODO: Implement when match events are tracked
            return await Task.FromResult(new List<PlayerDto>());
        }
    }
}
