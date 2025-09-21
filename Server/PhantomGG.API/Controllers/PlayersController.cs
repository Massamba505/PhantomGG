using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.Entities;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Implementations;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

/// <summary>
/// Controller for managing players and player statistics
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlayersController(
    IPlayerService playerService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly IPlayerService _playerService = playerService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Get all players (public access)
    /// </summary>
    /// <returns>List of all players</returns>
    /// <response code="200">Returns the list of players</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public ActionResult<ApiResponse> GetPlayers()
    {
        // Players are managed through teams - redirect to teams endpoint
        return Ok(new ApiResponse
        {
            Success = false,
            Data = null,
            Message = "Players are managed through teams. Use /api/teams/{teamId}/players instead."
        });
    }

    /// <summary>
    /// Get a specific player by ID
    /// </summary>
    /// <param name="id">The player ID</param>
    /// <returns>The player details</returns>
    /// <response code="200">Returns the player</response>
    /// <response code="404">If the player is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> GetPlayer(Guid id)
    {
        var player = await _playerService.GetByIdAsync(id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = player,
            Message = "Player retrieved successfully"
        });
    }

    /// <summary>
    /// Get all players for a specific team
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <returns>List of players in the team</returns>
    /// <response code="200">Returns the list of team players</response>
    /// <response code="404">If the team is not found</response>
    [HttpGet("team/{teamId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public ActionResult<ApiResponse> GetPlayersByTeam(Guid teamId)
    {
        // Players are managed through teams - redirect to teams endpoint
        return Ok(new ApiResponse
        {
            Success = false,
            Data = null,
            Message = $"Use /api/teams/{teamId}/players instead."
        });
    }

    /// <summary>
    /// Get all players for a specific tournament
    /// </summary>
    /// <param name="tournamentId">The tournament ID</param>
    /// <returns>List of players in the tournament</returns>
    /// <response code="200">Returns the list of tournament players</response>
    /// <response code="404">If the tournament is not found</response>
    [HttpGet("tournament/{tournamentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public ActionResult<ApiResponse> GetPlayersByTournament(Guid tournamentId)
    {
        // Tournament players are accessed through teams
        return Ok(new ApiResponse
        {
            Success = false,
            Data = null,
            Message = $"Tournament players are accessed through teams. Use /api/teams?tournamentId={tournamentId} to get teams, then /api/teams/{{teamId}}/players for each team."
        });
    }

    /// <summary>
    /// Search players with filters
    /// </summary>
    /// <param name="searchTerm">Term to search in player names</param>
    /// <param name="teamId">Filter players by team ID</param>
    /// <param name="tournamentId">Filter players by tournament ID</param>
    /// <param name="position">Filter players by position</param>
    /// <param name="status">Filter players by status</param>
    /// <param name="pageNumber">Page number for pagination (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>List of players matching the search criteria</returns>
    /// <response code="200">Returns the list of matching players</response>
    /// <response code="400">If the search parameters are invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(400)]
    public ActionResult<ApiResponse> SearchPlayers(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? teamId = null,
        [FromQuery] Guid? tournamentId = null,
        [FromQuery] string? position = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Player search is available through teams
        return Ok(new ApiResponse
        {
            Success = false,
            Data = null,
            Message = "Player search is available through teams. Use /api/teams search with specific team filtering."
        });
    }

    /// <summary>
    /// Get top scorers for a tournament
    /// </summary>
    /// <param name="tournamentId">The tournament ID</param>
    /// <param name="limit">Maximum number of players to return (default: 10)</param>
    /// <returns>List of top scoring players</returns>
    /// <response code="200">Returns the list of top scorers</response>
    /// <response code="404">If the tournament is not found</response>
    [HttpGet("tournament/{tournamentId:guid}/top-scorers")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public ActionResult<ApiResponse> GetTopScorers(Guid tournamentId, [FromQuery] int limit = 10)
    {
        // Statistics features will be implemented in future phases
        return Ok(new ApiResponse
        {
            Success = false,
            Data = null,
            Message = "Player statistics features will be implemented in a future phase"
        });
    }

    /// <summary>
    /// Get players with most assists for a tournament
    /// </summary>
    /// <param name="tournamentId">The tournament ID</param>
    /// <param name="limit">Maximum number of players to return (default: 10)</param>
    /// <returns>List of players with most assists</returns>
    /// <response code="200">Returns the list of top assist providers</response>
    /// <response code="404">If the tournament is not found</response>
    [HttpGet("tournament/{tournamentId:guid}/top-assists")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public ActionResult<ApiResponse> GetTopAssists(Guid tournamentId, [FromQuery] int limit = 10)
    {
        // Statistics features will be implemented in future phases
        return Ok(new ApiResponse
        {
            Success = false,
            Data = null,
            Message = "Player statistics features will be implemented in a future phase"
        });
    }

    /// <summary>
    /// Create a new player
    /// </summary>
    /// <param name="createDto">Player creation data</param>
    /// <returns>The created player</returns>
    /// <response code="201">Returns the newly created player</response>
    /// <response code="400">If the player data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to create players</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse>> CreatePlayer([FromBody] CreatePlayerDto createDto)
    {
        var user = _currentUserService.GetCurrentUser();

        //var team = await _teamService.GetByIdAsync(teamId);

        //if (team.UserId != userId)
        //{
        //    throw new UnauthorizedException("You don't have permission to add players to this team.");
        //}

        //var currentTeamPlayerCount = await _playerRepository.GetPlayerCountByTeamAsync(teamId);
        //if (currentTeamPlayerCount >= 11)
        //{
        //    throw new InvalidOperationException("Team has reached maximum player limit (11 players).");
        //}
        var player = await _playerService.CreateAsync(createDto);
        return CreatedAtAction(
            nameof(GetPlayer),
            new { id = player.Id },
            new ApiResponse
            {
                Success = true,
                Data = player,
                Message = "Player created successfully"
            });
    }

    /// <summary>
    /// Update a player
    /// </summary>
    /// <param name="id">The player ID to update</param>
    /// <param name="updateDto">Player update data</param>
    /// <returns>The updated player</returns>
    /// <response code="200">Returns the updated player</response>
    /// <response code="400">If the update data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to update this player</response>
    /// <response code="404">If the player is not found</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> UpdatePlayer(Guid id, [FromBody] UpdatePlayerDto updateDto)
    {
        var user = _currentUserService.GetCurrentUser();

        //var team = await _teamRepository.GetByIdAsync(existingPlayer.TeamId);
        //if (team == null)
        //    throw new ArgumentException("Team not found.");

        //// Only team owner can update players
        //if (team.UserId != userId)
        //    throw new UnauthorizedException("You don't have permission to update this player.");
        var player = await _playerService.UpdateAsync(updateDto, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = player,
            Message = "Player updated successfully"
        });
    }

    /// <summary>
    /// Delete a player
    /// </summary>
    /// <param name="id">The player ID to delete</param>
    /// <returns>Confirmation of deletion</returns>
    /// <response code="200">Player deleted successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to delete this player</response>
    /// <response code="404">If the player is not found</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiResponse>> DeletePlayer(Guid id)
    {
        var user = _currentUserService.GetCurrentUser();
        //validate user is owner of the team the player belongs to
        await _playerService.DeleteAsync(id);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Player deleted successfully"
        });
    }

}
