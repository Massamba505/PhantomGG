using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Team;
using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Guid.Empty;
        }
        return userId;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetTeams()
    {
        var teams = await _teamService.GetAllAsync();
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Teams retrieved successfully"
        });
    }

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

    [HttpGet("tournament/{tournamentId:guid}")]
    public async Task<ActionResult<ApiResponse>> GetTeamsByTournament(Guid tournamentId)
    {
        var teams = await _teamService.GetByTournamentAsync(tournamentId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Teams retrieved successfully"
        });
    }

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse>> SearchTeams([FromBody] TeamSearchDto searchDto)
    {
        var teams = await _teamService.SearchAsync(searchDto);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Search completed successfully"
        });
    }

    [HttpGet("my-teams")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetMyTeams()
    {
        var userId = GetCurrentUserId();
        var teams = await _teamService.GetByLeaderAsync(userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Your teams retrieved successfully"
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateTeam([FromBody] CreateTeamDto createDto)
    {
        var userId = GetCurrentUserId();
        var team = await _teamService.CreateAsync(createDto, userId);
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

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateTeam(Guid id, [FromBody] UpdateTeamDto updateDto)
    {
        var userId = GetCurrentUserId();
        var team = await _teamService.UpdateAsync(id, updateDto, userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = team,
            Message = "Team updated successfully"
        });
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteTeam(Guid id)
    {
        var userId = GetCurrentUserId();
        await _teamService.DeleteAsync(id, userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team deleted successfully"
        });
    }
}
