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
    /// Search or list teams
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<TeamDto>>> GetTeams([FromQuery] TeamQuery query)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        Guid? userId = currentUser?.Role == UserRoles.User ? currentUser?.Id : null;

        var teams = await _teamService.SearchAsync(query, userId);
        return Ok(teams);
    }

    /// <summary>
    /// Get team details by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeamDto>> GetTeam(Guid id)
    {
        var team = await _teamService.GetByIdAsync(id);
        return Ok(team);
    }

    /// <summary>
    /// Create new team
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TeamDto>> CreateTeam([FromForm] CreateTeamDto createDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var team = await _teamService.CreateAsync(createDto, currentUser.Id);

        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
    }

    /// <summary>
    /// Update team details
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<TeamDto>> UpdateTeam(Guid id, [FromForm] UpdateTeamDto updateDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var team = await _teamService.UpdateAsync(id, updateDto, currentUser.Id);
        return Ok(team);
    }

    /// <summary>
    /// Delete team
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteTeam(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _teamService.DeleteAsync(id, currentUser.Id);
        return NoContent();
    }

    /// <summary>
    /// Get team players
    /// </summary>
    [HttpGet("{id:guid}/players")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetTeamPlayers(Guid id)
    {
        var players = await _teamService.GetTeamPlayersAsync(id);
        return Ok(players);
    }

    /// <summary>
    /// Add player to team
    /// </summary>
    [HttpPost("{id:guid}/players")]
    [Authorize]
    public async Task<ActionResult<PlayerDto>> AddPlayerToTeam(Guid id, [FromForm] CreatePlayerDto playerDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var player = await _teamService.AddPlayerToTeamAsync(id, playerDto, currentUser.Id);

        return CreatedAtAction(nameof(GetTeamPlayers), new { id }, player);
    }

    /// <summary>
    /// Update player in team
    /// </summary>
    [HttpPatch("{teamId:guid}/players/{playerId:guid}")]
    [Authorize]
    public async Task<ActionResult<PlayerDto>> UpdateTeamPlayer(
        Guid teamId,
        Guid playerId,
        [FromForm] UpdatePlayerDto updateDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var player = await _teamService.UpdateTeamPlayerAsync(teamId, playerId, updateDto, currentUser.Id);
        return Ok(player);
    }

    /// <summary>
    /// Remove player from team
    /// </summary>
    [HttpDelete("{teamId:guid}/players/{playerId:guid}")]
    [Authorize]
    public async Task<ActionResult> RemovePlayerFromTeam(Guid teamId, Guid playerId)
    {
        var currentUser = _currentUserService.GetCurrentUser()!;
        await _teamService.RemovePlayerFromTeamAsync(teamId, playerId, currentUser.Id);
        return NoContent();
    }
}
