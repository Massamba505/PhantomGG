using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Team;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController(
    ITeamService teamService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITeamService _teamService = teamService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

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
    public async Task<ActionResult<ApiResponse>> SearchTeams([FromQuery] TeamSearchDto searchDto)
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
        var user = _currentUserService.GetCurrentUser();
        var teams = await _teamService.GetByLeaderAsync(user.Id);
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

    [HttpPut("{id:guid}")]
    [Authorize]
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

    [HttpDelete("{id:guid}")]
    [Authorize]
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
}
