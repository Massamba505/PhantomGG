using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

/// <summary>
/// Team management controller with role-based access
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TeamsController(
    ITeamService teamService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITeamService _teamService = teamService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    #region Public Team Operations (Guest + User)

    /// <summary>
    /// Get all teams with search and filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetTeams([FromQuery] TeamSearchDto searchDto)
    {
        var teams = await _teamService.SearchAsync(searchDto);
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

    #endregion

    #region User Operations (Team Management)

    /// <summary>
    /// Get current user's teams
    /// </summary>
    [HttpGet("my-teams")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<ApiResponse>> GetMyTeams()
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var teams = await _teamService.GetMyTeamsAsync(currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Your teams retrieved successfully"
        });
    }

    /// <summary>
    /// Create new team
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateTeam([FromBody] CreateTeamDto createDto)
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
    public async Task<ActionResult<ApiResponse>> UpdateTeam(Guid id, [FromBody] UpdateTeamDto updateDto)
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
    /// Upload team logo
    /// </summary>
    [HttpPost("{id:guid}/logo")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UploadTeamLogo(Guid id, IFormFile file)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var logoUrl = await _teamService.UploadLogoAsync(id, file, currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { logoUrl },
            Message = "Team logo uploaded successfully"
        });
    }

    #endregion

    #region User Player Management (Team Members)

    /// <summary>
    /// Add player to team
    /// </summary>
    [HttpPost("{id:guid}/players")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> AddPlayerToTeam(Guid id, [FromBody] CreatePlayerDto playerDto)
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
    public async Task<ActionResult<ApiResponse>> UpdateTeamPlayer(Guid teamId, Guid playerId, [FromBody] UpdatePlayerDto updateDto)
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

    /// <summary>
    /// Upload player photo
    /// </summary>
    [HttpPost("{teamId:guid}/players/{playerId:guid}/photo")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UploadPlayerPhoto(Guid teamId, Guid playerId, IFormFile file)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var photoUrl = await _teamService.UploadPlayerPhotoAsync(teamId, playerId, file, currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { photoUrl },
            Message = "Player photo uploaded successfully"
        });
    }

    #endregion
}
