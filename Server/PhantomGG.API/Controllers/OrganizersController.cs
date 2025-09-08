using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.Common;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.DTOs.Team;
using PhantomGG.API.DTOs.Player;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizersController(
    ITournamentService tournamentService,
    ITeamService teamService,
    IPlayerService playerService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITournamentService _tournamentService = tournamentService;
    private readonly ITeamService _teamService = teamService;
    private readonly IPlayerService _playerService = playerService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    #region Tournament Management

    /// <summary>
    /// Get all tournaments organized by the current user
    /// </summary>
    [HttpGet("my-tournaments")]
    public async Task<ActionResult<ApiResponse>> GetMyTournaments()
    {
        var user = _currentUserService.GetCurrentUser();
        var tournaments = await _tournamentService.GetByOrganizerAsync(user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Your tournaments retrieved successfully"
        });
    }

    /// <summary>
    /// Create a new tournament
    /// </summary>
    /// <param name="createDto">Tournament creation data</param>
    [HttpPost("tournaments")]
    public async Task<ActionResult<ApiResponse>> CreateTournament([FromBody] CreateTournamentDto createDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.CreateAsync(createDto, user.Id);
        return CreatedAtAction(
            nameof(GetTournamentDetails),
            new { tournamentId = tournament.Id },
            new ApiResponse
            {
                Success = true,
                Data = tournament,
                Message = "Tournament created successfully"
            });
    }

    /// <summary>
    /// Update a tournament you organize
    /// </summary>
    /// <param name="tournamentId">The tournament ID to update</param>
    /// <param name="updateDto">Tournament update data</param>
    [HttpPut("tournaments/{tournamentId:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateTournament(Guid tournamentId, [FromBody] UpdateTournamentDto updateDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.UpdateAsync(tournamentId, updateDto, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournament,
            Message = "Tournament updated successfully"
        });
    }

    /// <summary>
    /// Delete a tournament you organize
    /// </summary>
    /// <param name="tournamentId">The tournament ID to delete</param>
    [HttpDelete("tournaments/{tournamentId:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteTournament(Guid tournamentId)
    {
        var user = _currentUserService.GetCurrentUser();
        await _tournamentService.DeleteAsync(tournamentId, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Tournament deleted successfully"
        });
    }

    /// <summary>
    /// Get detailed information about a specific tournament you organize
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}")]
    public async Task<ActionResult<ApiResponse>> GetTournamentDetails(Guid tournamentId)
    {
        var tournament = await _tournamentService.GetByIdAsync(tournamentId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournament,
            Message = "Tournament details retrieved successfully"
        });
    }

    /// <summary>
    /// Upload tournament banner
    /// </summary>
    [HttpPost("tournaments/{tournamentId:guid}/banner")]
    public async Task<ActionResult<ApiResponse>> UploadTournamentBanner(Guid tournamentId, IFormFile file)
    {
        var user = _currentUserService.GetCurrentUser();
        var bannerUrl = await _tournamentService.UploadTournamentBannerAsync(tournamentId, file, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { bannerUrl },
            Message = "Tournament banner uploaded successfully"
        });
    }

    /// <summary>
    /// Upload tournament banner
    /// </summary>
    [HttpPost("tournaments/{tournamentId:guid}/logo")]
    public async Task<ActionResult<ApiResponse>> UploadTournamentlogo(Guid tournamentId, IFormFile file)
    {
        var user = _currentUserService.GetCurrentUser();
        var bannerUrl = await _tournamentService.UploadTournamentLogoAsync(tournamentId, file, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { bannerUrl },
            Message = "Tournament logo uploaded successfully"
        });
    }

    #endregion

    #region Team Management

    /// <summary>
    /// Get all teams in a tournament you organize
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/teams")]
    public async Task<ActionResult<ApiResponse>> GetTournamentTeams(Guid tournamentId)
    {
        var teams = await _teamService.GetByTournamentAsync(tournamentId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Tournament teams retrieved successfully"
        });
    }

    /// <summary>
    /// Get detailed information about a specific team
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/teams/{teamId:guid}")]
    public async Task<ActionResult<ApiResponse>> GetTeamDetails(Guid tournamentId, Guid teamId)
    {
        var team = await _teamService.GetByIdAsync(teamId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = team,
            Message = "Team details retrieved successfully"
        });
    }

    /// <summary>
    /// Approve a team registration
    /// </summary>
    [HttpPost("tournaments/{tournamentId:guid}/teams/{teamId:guid}/approve")]
    public async Task<ActionResult<ApiResponse>> ApproveTeam(Guid tournamentId, Guid teamId)
    {
        var user = _currentUserService.GetCurrentUser();
        await _teamService.ApproveTeamAsync(teamId, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team approved successfully"
        });
    }

    /// <summary>
    /// Reject a team registration
    /// </summary>
    [HttpPost("tournaments/{tournamentId:guid}/teams/{teamId:guid}/reject")]
    public async Task<ActionResult<ApiResponse>> RejectTeam(Guid tournamentId, Guid teamId, [FromBody] string? reason = null)
    {
        var user = _currentUserService.GetCurrentUser();
        await _teamService.RejectTeamAsync(teamId, user.Id, reason);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team rejected successfully"
        });
    }

    /// <summary>
    /// Get teams by registration status
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/teams/status/{status}")]
    public async Task<ActionResult<ApiResponse>> GetTeamsByStatus(Guid tournamentId, string status)
    {
        var teams = await _teamService.GetByRegistrationStatusAsync(tournamentId, status);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = $"Teams with status '{status}' retrieved successfully"
        });
    }

    /// <summary>
    /// Update team information (organizer override)
    /// </summary>
    [HttpPut("tournaments/{tournamentId:guid}/teams/{teamId:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateTeam(Guid tournamentId, Guid teamId, [FromBody] UpdateTeamDto updateDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var team = await _teamService.UpdateAsync(teamId, updateDto, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = team,
            Message = "Team updated successfully"
        });
    }

    /// <summary>
    /// Remove a team from tournament
    /// </summary>
    [HttpDelete("tournaments/{tournamentId:guid}/teams/{teamId:guid}")]
    public async Task<ActionResult<ApiResponse>> RemoveTeam(Guid tournamentId, Guid teamId)
    {
        var user = _currentUserService.GetCurrentUser();
        await _teamService.DeleteAsync(teamId, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team removed from tournament successfully"
        });
    }

    #endregion

    #region Player Management

    /// <summary>
    /// Get all players in a tournament you organize
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/players")]
    public async Task<ActionResult<ApiResponse>> GetTournamentPlayers(Guid tournamentId)
    {
        var players = await _playerService.GetByTournamentAsync(tournamentId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = players,
            Message = "Tournament players retrieved successfully"
        });
    }

    /// <summary>
    /// Get all players for a specific team
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/teams/{teamId:guid}/players")]
    public async Task<ActionResult<ApiResponse>> GetTeamPlayers(Guid tournamentId, Guid teamId)
    {
        var players = await _playerService.GetByTeamAsync(teamId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = players,
            Message = "Team players retrieved successfully"
        });
    }

    /// <summary>
    /// Get detailed information about a specific player
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/teams/{teamId:guid}/players/{playerId:guid}")]
    public async Task<ActionResult<ApiResponse>> GetPlayerDetails(Guid tournamentId, Guid teamId, Guid playerId)
    {
        var player = await _playerService.GetByIdAsync(playerId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = player,
            Message = "Player details retrieved successfully"
        });
    }

    /// <summary>
    /// Add a player to a team (organizer override)
    /// </summary>
    [HttpPost("tournaments/{tournamentId:guid}/teams/{teamId:guid}/players")]
    public async Task<ActionResult<ApiResponse>> AddPlayerToTeam(Guid tournamentId, Guid teamId, [FromBody] CreatePlayerDto createDto)
    {
        var user = _currentUserService.GetCurrentUser();
        // Ensure the player is assigned to the correct team
        createDto.TeamId = teamId;
        var player = await _playerService.CreateAsync(createDto, user.Id);
        return CreatedAtAction(
            nameof(GetPlayerDetails),
            new { tournamentId, teamId, playerId = player.Id },
            new ApiResponse
            {
                Success = true,
                Data = player,
                Message = "Player added to team successfully"
            });
    }

    /// <summary>
    /// Update player information (organizer override)
    /// </summary>
    [HttpPut("tournaments/{tournamentId:guid}/teams/{teamId:guid}/players/{playerId:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdatePlayer(Guid tournamentId, Guid teamId, Guid playerId, [FromBody] UpdatePlayerDto updateDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var player = await _playerService.UpdateAsync(playerId, updateDto, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = player,
            Message = "Player updated successfully"
        });
    }

    /// <summary>
    /// Remove a player from a team
    /// </summary>
    [HttpDelete("tournaments/{tournamentId:guid}/teams/{teamId:guid}/players/{playerId:guid}")]
    public async Task<ActionResult<ApiResponse>> RemovePlayer(Guid tournamentId, Guid teamId, Guid playerId)
    {
        var user = _currentUserService.GetCurrentUser();
        await _playerService.DeleteAsync(playerId, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Player removed from team successfully"
        });
    }

    #endregion

    #region Tournament Statistics

    /// <summary>
    /// Get tournament statistics overview
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/statistics")]
    public async Task<ActionResult<ApiResponse>> GetTournamentStatistics(Guid tournamentId)
    {
        var tournament = await _tournamentService.GetByIdAsync(tournamentId);
        var teams = await _teamService.GetByTournamentAsync(tournamentId);
        var players = await _playerService.GetByTournamentAsync(tournamentId);

        var statistics = new
        {
            tournament.Id,
            tournament.Name,
            tournament.Status,
            TotalTeams = teams.Count(),
            RegisteredTeams = teams.Count(t => t.RegistrationStatus == "Approved"),
            PendingTeams = teams.Count(t => t.RegistrationStatus == "Pending"),
            RejectedTeams = teams.Count(t => t.RegistrationStatus == "Rejected"),
            TotalPlayers = players.Count(),
            ActivePlayers = players.Count(p => p.IsActive), // Using IsActive instead of Status
            tournament.CreatedAt,
            tournament.StartDate
        };

        return Ok(new ApiResponse
        {
            Success = true,
            Data = statistics,
            Message = "Tournament statistics retrieved successfully"
        });
    }

    /// <summary>
    /// Get top scorers for tournament
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/top-scorers")]
    public async Task<ActionResult<ApiResponse>> GetTopScorers(Guid tournamentId, [FromQuery] int limit = 10)
    {
        var players = await _playerService.GetTopScorersAsync(tournamentId, limit);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = players,
            Message = "Top scorers retrieved successfully"
        });
    }

    /// <summary>
    /// Get players with most assists for tournament
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/top-assists")]
    public async Task<ActionResult<ApiResponse>> GetTopAssists(Guid tournamentId, [FromQuery] int limit = 10)
    {
        var players = await _playerService.GetTopAssistsAsync(tournamentId, limit);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = players,
            Message = "Top assists retrieved successfully"
        });
    }

    #endregion

    #region Search and Filters

    /// <summary>
    /// Search teams within a tournament
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/teams/search")]
    public async Task<ActionResult<ApiResponse>> SearchTeams(
        Guid tournamentId,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var searchDto = new TeamSearchDto
        {
            SearchTerm = searchTerm,
            TournamentId = tournamentId,
            RegistrationStatus = string.IsNullOrEmpty(status) ? null : Enum.Parse<TeamRegistrationStatus>(status, true),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var teams = await _teamService.SearchAsync(searchDto);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Team search completed successfully"
        });
    }

    /// <summary>
    /// Search players within a tournament
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/players/search")]
    public async Task<ActionResult<ApiResponse>> SearchPlayers(
        Guid tournamentId,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? teamId = null,
        [FromQuery] string? position = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var searchDto = new PlayerSearchDto
        {
            SearchTerm = searchTerm,
            TournamentId = tournamentId,
            TeamId = teamId,
            Position = string.IsNullOrEmpty(position) ? null : Enum.Parse<PlayerPosition>(position, true),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var players = await _playerService.SearchAsync(searchDto);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = players,
            Message = "Player search completed successfully"
        });
    }

    #endregion
}
