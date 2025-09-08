using PhantomGG.API.Common;
using PhantomGG.API.DTOs.Player;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Mappings;

namespace PhantomGG.API.Services.Implementations
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ICurrentUserService _currentUserService;

        public PlayerService(
            IPlayerRepository playerRepository,
            ITeamRepository teamRepository,
            ITournamentRepository tournamentRepository,
            ICurrentUserService currentUserService)
        {
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _tournamentRepository = tournamentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<PlayerDto>> GetAllAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            return players.Select(p => p.ToPlayerDto());
        }

        public async Task<PlayerDto> GetByIdAsync(Guid id)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                throw new ArgumentException("Player not found.");

            return player.ToPlayerDto();
        }

        public async Task<IEnumerable<PlayerDto>> GetByTeamAsync(Guid teamId)
        {
            var players = await _playerRepository.GetByTeamAsync(teamId);
            return players.Select(p => p.ToPlayerDto());
        }

        public async Task<IEnumerable<PlayerDto>> GetByTournamentAsync(Guid tournamentId)
        {
            // Get all teams in the tournament, then get all players for those teams
            var teams = await _teamRepository.GetByTournamentAsync(tournamentId);
            var teamIds = teams.Select(t => t.Id).ToList();

            var allPlayers = await _playerRepository.GetAllAsync();
            var tournamentPlayers = allPlayers.Where(p => teamIds.Contains(p.TeamId));

            return tournamentPlayers.Select(p => p.ToPlayerDto());
        }

        public async Task<IEnumerable<PlayerDto>> SearchAsync(PlayerSearchDto searchDto)
        {
            // For MVP, implement basic search
            var allPlayers = await _playerRepository.GetAllAsync();
            var filteredPlayers = allPlayers.AsQueryable();

            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                filteredPlayers = filteredPlayers.Where(p =>
                    p.FirstName.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.LastName.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (searchDto.TeamId.HasValue)
            {
                filteredPlayers = filteredPlayers.Where(p => p.TeamId == searchDto.TeamId.Value);
            }

            if (searchDto.Position != null)
            {
                var positionString = searchDto.Position.ToString();
                filteredPlayers = filteredPlayers.Where(p => p.Position == positionString);
            }

            return filteredPlayers.Select(p => p.ToPlayerDto());
        }

        public async Task<PlayerDto> CreateAsync(CreatePlayerDto createDto, Guid userId)
        {
            // Validate user is authenticated
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to create a player.");

            // Validate team exists
            var team = await _teamRepository.GetByIdAsync(createDto.TeamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            // Check permissions - team manager or tournament organizer can add players
            var currentUser = _currentUserService.GetCurrentUser();
            var tournament = await _tournamentRepository.GetByIdAsync(team.TournamentId);

            bool isTeamManager = team.ManagerEmail == currentUser.Email;
            bool isTournamentOrganizer = tournament?.OrganizerId == currentUser.Id;

            if (!isTeamManager && !isTournamentOrganizer)
                throw new UnauthorizedAccessException("You don't have permission to add players to this team.");

            // Check team player limit
            var currentPlayerCount = await _playerRepository.GetPlayerCountByTeamAsync(createDto.TeamId);
            if (tournament != null && currentPlayerCount >= tournament.MaxPlayersPerTeam)
                throw new InvalidOperationException("Team has reached maximum player limit.");

            var player = createDto.ToPlayer();
            var createdPlayer = await _playerRepository.CreateAsync(player);

            return createdPlayer.ToPlayerDto();
        }

        public async Task<PlayerDto> UpdateAsync(Guid id, UpdatePlayerDto updateDto, Guid userId)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
                throw new ArgumentException("Player not found.");

            // Check permissions
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to update a player.");

            var currentUser = _currentUserService.GetCurrentUser();
            var team = await _teamRepository.GetByIdAsync(existingPlayer.TeamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            var tournament = await _tournamentRepository.GetByIdAsync(team.TournamentId);

            bool isTeamManager = team.ManagerEmail == currentUser.Email;
            bool isTournamentOrganizer = tournament?.OrganizerId == currentUser.Id;

            if (!isTeamManager && !isTournamentOrganizer)
                throw new UnauthorizedAccessException("You don't have permission to update this player.");

            existingPlayer.UpdateFromDto(updateDto);
            var updatedPlayer = await _playerRepository.UpdateAsync(existingPlayer);

            return updatedPlayer.ToPlayerDto();
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                return;

            // Check permissions
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to delete a player.");

            var currentUser = _currentUserService.GetCurrentUser();
            var team = await _teamRepository.GetByIdAsync(player.TeamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            var tournament = await _tournamentRepository.GetByIdAsync(team.TournamentId);

            bool isTeamManager = team.ManagerEmail == currentUser.Email;
            bool isTournamentOrganizer = tournament?.OrganizerId == currentUser.Id;

            if (!isTeamManager && !isTournamentOrganizer)
                throw new UnauthorizedAccessException("You don't have permission to delete this player.");

            // Check if tournament has started
            if (tournament != null && tournament.StartDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot delete players from a tournament that has already started.");

            await _playerRepository.DeleteAsync(id);
        }

        public async Task<bool> IsPlayerOwnedByUserAsync(Guid playerId, Guid userId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null) return false;

            if (!_currentUserService.IsAuthenticated()) return false;

            var currentUser = _currentUserService.GetCurrentUser();
            var team = await _teamRepository.GetByIdAsync(player.TeamId);

            return team?.ManagerEmail == currentUser.Email;
        }

        public async Task<string> UploadPlayerPhotoAsync(Guid playerId, IFormFile file, Guid userId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null)
                throw new ArgumentException("Player not found.");

            // Check permissions
            if (!await IsPlayerOwnedByUserAsync(playerId, userId))
                throw new UnauthorizedAccessException("You don't have permission to update this player's photo.");

            // For MVP, return a placeholder URL
            // TODO: Implement actual file upload service
            return "/images/player-photos/placeholder.png";
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
