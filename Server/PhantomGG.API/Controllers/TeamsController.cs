using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

/// <summary>
/// Controller for managing teams
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TeamsController(
    ITeamService teamService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITeamService _teamService = teamService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Get all teams with pagination (public access)
    /// </summary>
    /// <param name="pageNumber">Page number for pagination (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>A paginated list of teams</returns>
    /// <response code="200">Returns the list of teams</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<ActionResult<ApiResponse>> GetTeams([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var teams = await _teamService.GetAllAsync();
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Teams retrieved successfully"
        });
    }

    /// <summary>
    /// Get a specific team by ID
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <returns>The team details</returns>
    /// <response code="200">Returns the team</response>
    /// <response code="404">If the team is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> GetTeam(Guid id)
    {
        var team = await _teamService.GetByIdAsync(id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = team,
            Message = "Team retrieved successfully"
        });
    }

    /// <summary>
    /// Search teams with filters
    /// </summary>
    /// <param name="searchTerm">Term to search in team names</param>
    /// <param name="tournamentId">Filter teams by tournament ID</param>
    /// <param name="status">Filter teams by registration status</param>
    /// <param name="pageNumber">Page number for pagination (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>List of teams matching the search criteria</returns>
    /// <response code="200">Returns the list of matching teams</response>
    /// <response code="400">If the search parameters are invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ApiResponse>> SearchTeams(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? tournamentId = null,
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
            Message = "Search completed successfully"
        });
    }

    /// <summary>
    /// Get teams for a specific tournament
    /// </summary>
    /// <param name="tournamentId">The tournament ID</param>
    /// <returns>List of teams registered for the tournament</returns>
    /// <response code="200">Returns the list of tournament teams</response>
    /// <response code="404">If the tournament is not found</response>
    [HttpGet("by-tournament/{tournamentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> GetTeamsByTournament(Guid tournamentId)
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
    /// Get teams managed by current user
    /// </summary>
    /// <returns>List of teams where the current user is the leader</returns>
    /// <response code="200">Returns the list of user's teams</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet("my-teams")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse>> GetMyTeams()
    {
        var user = _currentUserService.GetCurrentUser();
        var teams = await _teamService.GetByLeaderAsync(user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Your teams retrieved successfully"
        });
    }

    /// <summary>
    /// Create a new team
    /// </summary>
    /// <param name="createDto">Team creation data</param>
    /// <returns>The created team</returns>
    /// <response code="201">Returns the newly created team</response>
    /// <response code="400">If the team data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="409">If a team with the same name already exists</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(409)]
    public async Task<ActionResult<ApiResponse>> CreateTeam([FromBody] CreateTeamDto createDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var team = await _teamService.CreateAsync(createDto, user.Id);
        return CreatedAtAction(
            nameof(GetTeam),
            new { id = team.Id },
            new ApiResponse
            {
                Success = true,
                Data = team,
                Message = "Team created successfully"
            });
    }

    /// <summary>
    /// Update a team
    /// </summary>
    /// <param name="id">The team ID to update</param>
    /// <param name="updateDto">Team update data</param>
    /// <returns>The updated team</returns>
    /// <response code="200">Returns the updated team</response>
    /// <response code="400">If the update data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to update this team</response>
    /// <response code="404">If the team is not found</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> UpdateTeam(Guid id, [FromBody] UpdateTeamDto updateDto)
    {
        var user = _currentUserService.GetCurrentUser();
        var team = await _teamService.UpdateAsync(id, updateDto, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = team,
            Message = "Team updated successfully"
        });
    }

    /// <summary>
    /// Delete a team
    /// </summary>
    /// <param name="id">The team ID to delete</param>
    /// <returns>Confirmation of deletion</returns>
    /// <response code="200">Team deleted successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to delete this team</response>
    /// <response code="404">If the team is not found</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> DeleteTeam(Guid id)
    {
        var user = _currentUserService.GetCurrentUser();
        await _teamService.DeleteAsync(id, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team deleted successfully"
        });
    }

    /// <summary>
    /// Upload team logo
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="file">The logo image file</param>
    /// <returns>The URL of the uploaded logo</returns>
    /// <response code="200">Logo uploaded successfully</response>
    /// <response code="400">If no file is provided or file is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to update this team</response>
    /// <response code="404">If the team is not found</response>
    [HttpPost("{id:guid}/logo")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> UploadTeamLogo(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "No file provided"
            });
        }

        var user = _currentUserService.GetCurrentUser();
        var logoUrl = await _teamService.UploadTeamLogoAsync(id, file, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { logoUrl },
            Message = "Team logo uploaded successfully"
        });
    }

    /// <summary>
    /// Get team statistics
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <returns>Team statistics including registration info and player count</returns>
    /// <response code="200">Returns the team statistics</response>
    /// <response code="404">If the team is not found</response>
    [HttpGet("{id:guid}/statistics")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> GetTeamStatistics(Guid id)
    {
        var team = await _teamService.GetByIdAsync(id);

        // Basic statistics for MVP
        var statistics = new
        {
            team.Id,
            team.Name,
            team.RegistrationStatus,
            team.NumberOfPlayers,
            team.RegistrationDate,
            team.CreatedAt
        };

        return Ok(new ApiResponse
        {
            Success = true,
            Data = statistics,
            Message = "Team statistics retrieved successfully"
        });
    }
}
