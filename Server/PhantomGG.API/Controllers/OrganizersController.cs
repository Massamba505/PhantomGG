using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.Common;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Organizer")]
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

    /// <summary>
    /// Get all tournaments organized by the current user
    /// </summary>
    [HttpGet("tournaments")]
    public async Task<ActionResult<ApiResponse>> GetMyTournaments()
    {
        var user = _currentUserService.GetCurrentUser();
        var tournaments = await _tournamentService.GetByOrganizerAsync(user.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Tournaments retrieved successfully"
        });
    }

    /// <summary>
    /// Get a specific tournament by ID
    /// </summary>
    [HttpGet("tournaments/{id:guid}")]
    public async Task<ActionResult<ApiResponse>> GetTournament(Guid id)
    {

        var tournament = await _tournamentService.GetByIdAsync(id);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournament,
            Message = "Tournament retrieved successfully"
        });
    }

    /// <summary>
    /// Create a new tournament
    /// </summary>
    [HttpPost("tournaments")]
    public async Task<ActionResult<ApiResponse>> CreateTournament([FromBody] CreateTournamentDto createDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.CreateAsync(createDto, user.Id);

        return CreatedAtAction(
            nameof(GetTournament),
            new { id = tournament.Id },
            new ApiResponse
            {
                Success = true,
                Data = tournament,
                Message = "Tournament created successfully"
            });
    }

    /// <summary>
    /// Update an existing tournament
    /// </summary>
    [HttpPut("tournaments/{id:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateTournament(Guid id, [FromBody] UpdateTournamentDto updateDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.UpdateAsync(id, updateDto, user.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournament,
            Message = "Tournament updated successfully"
        });
    }

    /// <summary>
    /// Delete a tournament
    /// </summary>
    [HttpDelete("tournaments/{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteTournament(Guid id)
    {
        var user = _currentUserService.GetCurrentUser();
        await _tournamentService.DeleteAsync(id, user.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Tournament deleted successfully"
        });
    }

    /// <summary>
    /// Upload tournament banner image
    /// </summary>
    [HttpPost("tournaments/{id:guid}/banner")]
    public async Task<ActionResult<ApiResponse>> UploadTournamentBanner(Guid id, IFormFile file)
    {
        var user = _currentUserService.GetCurrentUser();
        var bannerUrl = await _tournamentService.UploadTournamentBannerAsync(id, file, user.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { bannerUrl },
            Message = "Tournament banner uploaded successfully"
        });
    }

    /// <summary>
    /// Upload tournament logo image
    /// </summary>
    [HttpPost("tournaments/{id:guid}/logo")]
    public async Task<ActionResult<ApiResponse>> UploadTournamentLogo(Guid id, IFormFile file)
    {
        var user = _currentUserService.GetCurrentUser();
        var logoUrl = await _tournamentService.UploadTournamentLogoAsync(id, file, user.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { logoUrl },
            Message = "Tournament logo uploaded successfully"
        });
    }

    /// <summary>
    /// Search tournaments with filters and pagination
    /// </summary>
    [HttpGet("tournaments/search")]
    public async Task<ActionResult<ApiResponse>> SearchTournaments([FromQuery] TournamentSearchDto searchDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var tournaments = await _tournamentService.SearchWithPaginationAsync(searchDto, user.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Search completed successfully"
        });
    }

    /// <summary>
    /// Get all teams registered for a tournament
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
    /// Reject a team registration with optional reason
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
    /// Get teams by registration status (Pending, Approved, Rejected)
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
    /// Remove a team from the tournament
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

    /// <summary>
    /// Get all players participating in the tournament
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
    /// Get comprehensive tournament statistics and metrics
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
            ActivePlayers = players.Count(p => p.IsActive),
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
    /// Get top scoring players in the tournament
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
    /// Get players with most assists in the tournament
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

    /// <summary>
    /// Get complete tournament dashboard data
    /// </summary>
    [HttpGet("tournaments/{tournamentId:guid}/dashboard")]
    public async Task<ActionResult<ApiResponse>> GetDashboardData(Guid tournamentId)
    {
        var tournament = await _tournamentService.GetByIdAsync(tournamentId);
        var teams = await _teamService.GetByTournamentAsync(tournamentId);
        var players = await _playerService.GetByTournamentAsync(tournamentId);
        var topScorers = await _playerService.GetTopScorersAsync(tournamentId, 5);
        var topAssists = await _playerService.GetTopAssistsAsync(tournamentId, 5);

        var dashboardData = new
        {
            Tournament = tournament,
            Statistics = new
            {
                TotalTeams = teams.Count(),
                RegisteredTeams = teams.Count(t => t.RegistrationStatus == TeamRegistrationStatus.Approved.ToString()),
                PendingTeams = teams.Count(t => t.RegistrationStatus == TeamRegistrationStatus.Pending.ToString()),
                RejectedTeams = teams.Count(t => t.RegistrationStatus == TeamRegistrationStatus.Rejected.ToString()),
                TotalPlayers = players.Count(),
                ActivePlayers = players.Count(p => p.IsActive)
            },
            TopScorers = topScorers,
            TopAssists = topAssists,
            RecentTeams = teams.OrderByDescending(t => t.CreatedAt).Take(5)
        };

        return Ok(new ApiResponse
        {
            Success = true,
            Data = dashboardData,
            Message = "Dashboard data retrieved successfully"
        });
    }
}
