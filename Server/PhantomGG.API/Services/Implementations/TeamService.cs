using PhantomGG.API.DTOs.Team;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Mappings;
using PhantomGG.API.Common;

namespace PhantomGG.API.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;

        public TeamService(
            ITeamRepository teamRepository,
            ITournamentRepository tournamentRepository,
            ICurrentUserService currentUserService,
            IImageService imageService)
        {
            _teamRepository = teamRepository;
            _tournamentRepository = tournamentRepository;
            _currentUserService = currentUserService;
            _imageService = imageService;
        }

        public async Task<IEnumerable<TeamDto>> GetAllAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            return teams.Select(t => t.ToTeamDto());
        }

        public async Task<TeamDto> GetByIdAsync(Guid id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
                throw new ArgumentException("Team not found.");

            return team.ToTeamDto();
        }

        public async Task<IEnumerable<TeamDto>> GetByLeaderAsync(Guid leaderId)
        {
            // For MVP, we'll use the manager email/name as identifier
            // This would need to be improved for proper user management
            var teams = await _teamRepository.GetAllAsync();
            var userTeams = teams.Where(t => t.ManagerEmail != null &&
                                           _currentUserService.IsAuthenticated() &&
                                           _currentUserService.GetCurrentUser().Id == leaderId);
            return userTeams.Select(t => t.ToTeamDto());
        }

        public async Task<IEnumerable<TeamDto>> GetByTournamentAsync(Guid tournamentId)
        {
            var teams = await _teamRepository.GetByTournamentAsync(tournamentId);
            return teams.Select(t => t.ToTeamDto());
        }

        public async Task<IEnumerable<TeamDto>> SearchAsync(TeamSearchDto searchDto)
        {
            var teams = await _teamRepository.SearchAsync(searchDto);
            return teams.Select(t => t.ToTeamDto());
        }

        public async Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid leaderId)
        {
            // Validate user is authenticated
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to create a team.");

            // Validate tournament exists and registration is open
            var tournament = await _tournamentRepository.GetByIdAsync(createDto.TournamentId);
            if (tournament == null)
                throw new ArgumentException("Tournament not found.");

            // Check if registration is still open
            if (tournament.RegistrationDeadline.HasValue && tournament.RegistrationDeadline <= DateTime.UtcNow)
                throw new InvalidOperationException("Registration deadline has passed.");

            // Check if tournament hasn't started
            if (tournament.StartDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot register for a tournament that has already started.");

            // Check team name uniqueness in tournament
            if (await _teamRepository.TeamNameExistsInTournamentAsync(createDto.Name, createDto.TournamentId))
                throw new ArgumentException("A team with this name already exists in this tournament.");

            // Check if tournament has space for more teams
            var teamCount = await _tournamentRepository.GetTeamCountAsync(createDto.TournamentId);
            if (teamCount >= tournament.MaxTeams)
                throw new InvalidOperationException("Tournament is full.");

            var team = createDto.ToTeam();
            team.RegistrationDate = DateTime.UtcNow;
            team.RegistrationStatus = "Pending";

            var createdTeam = await _teamRepository.CreateAsync(team);
            return createdTeam.ToTeamDto();
        }

        public async Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto updateDto, Guid userId)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(id);
            if (existingTeam == null)
                throw new ArgumentException("Team not found.");

            // Check permissions - team manager or tournament organizer can update
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to update a team.");

            var currentUser = _currentUserService.GetCurrentUser();
            var tournament = await _tournamentRepository.GetByIdAsync(existingTeam.TournamentId);

            bool isTeamManager = existingTeam.ManagerEmail == currentUser.Email;
            bool isTournamentOrganizer = tournament?.OrganizerId == currentUser.Id;

            if (!isTeamManager && !isTournamentOrganizer)
                throw new UnauthorizedAccessException("You don't have permission to update this team.");

            // If updating name, check uniqueness
            if (!string.IsNullOrEmpty(updateDto.Name) && updateDto.Name != existingTeam.Name)
            {
                if (await _teamRepository.TeamNameExistsInTournamentAsync(updateDto.Name, existingTeam.TournamentId, existingTeam.Id))
                    throw new ArgumentException("A team with this name already exists in this tournament.");
            }

            existingTeam.UpdateFromDto(updateDto);
            var updatedTeam = await _teamRepository.UpdateAsync(existingTeam);
            return updatedTeam.ToTeamDto();
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
                return;

            // Check permissions
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to delete a team.");

            var currentUser = _currentUserService.GetCurrentUser();
            var tournament = await _tournamentRepository.GetByIdAsync(team.TournamentId);

            bool isTeamManager = team.ManagerEmail == currentUser.Email;
            bool isTournamentOrganizer = tournament?.OrganizerId == currentUser.Id;

            if (!isTeamManager && !isTournamentOrganizer)
                throw new UnauthorizedAccessException("You don't have permission to delete this team.");

            // Check if tournament has started
            if (tournament != null && tournament.StartDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot delete a team from a tournament that has already started.");

            await _teamRepository.DeleteAsync(id);
        }

        public async Task<string> UploadTeamLogoAsync(Guid teamId, IFormFile file, Guid userId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            // Check permissions
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to upload team logo.");

            var currentUser = _currentUserService.GetCurrentUser();
            if (team.ManagerEmail != currentUser.Email)
                throw new UnauthorizedAccessException("You don't have permission to update this team's logo.");

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

        public async Task<IEnumerable<TeamDto>> GetByRegistrationStatusAsync(Guid tournamentId, string status)
        {
            var teams = await _teamRepository.GetByTournamentAsync(tournamentId);
            var filteredTeams = teams.Where(team => team.RegistrationStatus.Equals(status, StringComparison.OrdinalIgnoreCase));
            return filteredTeams.Select(team => team.ToTeamDto());
        }

        public async Task ApproveTeamAsync(Guid teamId, Guid organizerId)
        {
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to approve teams.");

            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            var tournament = await _tournamentRepository.GetByIdAsync(team.TournamentId);
            if (tournament == null)
                throw new ArgumentException("Tournament not found.");

            var currentUser = _currentUserService.GetCurrentUser();
            if (tournament.OrganizerId != currentUser.Id)
                throw new UnauthorizedAccessException("You don't have permission to approve teams for this tournament.");

            // Update team status
            team.RegistrationStatus = "Approved";
            team.ApprovedDate = DateTime.UtcNow;

            await _teamRepository.UpdateAsync(team);
        }

        public async Task RejectTeamAsync(Guid teamId, Guid organizerId, string? reason = null)
        {
            if (!_currentUserService.IsAuthenticated())
                throw new UnauthorizedAccessException("You must be logged in to reject teams.");

            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            var tournament = await _tournamentRepository.GetByIdAsync(team.TournamentId);
            if (tournament == null)
                throw new ArgumentException("Tournament not found.");

            var currentUser = _currentUserService.GetCurrentUser();
            if (tournament.OrganizerId != currentUser.Id)
                throw new UnauthorizedAccessException("You don't have permission to reject teams for this tournament.");

            // Update team status
            team.RegistrationStatus = "Rejected";
            team.UpdatedAt = DateTime.UtcNow;
            // Note: In a full implementation, you might want to store the rejection reason

            await _teamRepository.UpdateAsync(team);
        }
    }
}
