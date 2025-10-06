using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController(
    ITeamService teamService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITeamService _teamService = teamService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// team search 
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetTeams([FromQuery] TeamSearchDto searchDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        Guid? userId = currentUser.Role == UserRoles.Organizer.ToString() ? currentUser?.Id : null;

        var teams = await _teamService.SearchAsync(searchDto, userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Teams retrieved successfully"
        });
    }

    /// <summary>
    /// Get team details by ID
    /// </summary>
    [HttpGet("{id:guid}")]
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
    /// Get team players
    /// </summary>
    [HttpGet("{id:guid}/players")]
    public async Task<ActionResult<ApiResponse>> GetTeamPlayers(Guid id)
    {
        var players = await _teamService.GetTeamPlayersAsync(id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = players,
            Message = "Team players retrieved successfully"
        });
    }

    /// <summary>
    /// Create new team
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateTeam([FromForm] CreateTeamDto createDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var team = await _teamService.CreateAsync(createDto, currentUser.Id);
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
    /// Update team details
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateTeam(Guid id, [FromForm] UpdateTeamDto updateDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var team = await _teamService.UpdateAsync(id, updateDto, currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = team,
            Message = "Team updated successfully"
        });
    }

    /// <summary>
    /// Delete team
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteTeam(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _teamService.DeleteAsync(id, currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team deleted successfully"
        });
    }

    /// <summary>
    /// Add player to team
    /// </summary>
    [HttpPost("{id:guid}/players")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> AddPlayerToTeam(Guid id, [FromForm] CreatePlayerDto playerDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var player = await _teamService.AddPlayerToTeamAsync(id, playerDto, currentUser.Id);
        return CreatedAtAction(
            nameof(GetTeamPlayers),
            new { id },
            new ApiResponse
            {
                Success = true,
                Data = player,
                Message = "Player added to team successfully"
            });
    }

    /// <summary>
    /// Update player in team
    /// </summary>
    [HttpPut("{teamId:guid}/players/{playerId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateTeamPlayer(Guid teamId, Guid playerId, [FromForm] UpdatePlayerDto updateDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var player = await _teamService.UpdateTeamPlayerAsync(teamId, playerId, updateDto, currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = player,
            Message = "Player updated successfully"
        });
    }

    /// <summary>
    /// Remove player from team
    /// </summary>
    [HttpDelete("{teamId:guid}/players/{playerId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RemovePlayerFromTeam(Guid teamId, Guid playerId)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _teamService.RemovePlayerFromTeamAsync(teamId, playerId, currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Player removed from team successfully"
        });
    }
}
