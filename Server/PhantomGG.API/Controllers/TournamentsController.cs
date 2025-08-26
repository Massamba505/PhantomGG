using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
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
    public async Task<ActionResult<ApiResponse>> GetTournaments()
    {
        var tournaments = await _tournamentService.GetAllAsync();
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Tournaments retrieved successfully"
        });
    }

    [HttpGet("{id:guid}")]
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

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse>> SearchTournaments([FromBody] TournamentSearchDto searchDto)
    {
        var tournaments = await _tournamentService.SearchAsync(searchDto);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Search completed successfully"
        });
    }

    [HttpGet("my-tournaments")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetMyTournaments()
    {
        var userId = GetCurrentUserId();
        var tournaments = await _tournamentService.GetByOrganizerAsync(userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Your tournaments retrieved successfully"
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateTournament([FromBody] CreateTournamentDto createDto)
    {
        var userId = GetCurrentUserId();
        var tournament = await _tournamentService.CreateAsync(createDto, userId);
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

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateTournament(Guid id, [FromBody] UpdateTournamentDto updateDto)
    {
        var userId = GetCurrentUserId();
        var tournament = await _tournamentService.UpdateAsync(id, updateDto, userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournament,
            Message = "Tournament updated successfully"
        });
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteTournament(Guid id)
    {
        var userId = GetCurrentUserId();
        await _tournamentService.DeleteAsync(id, userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Tournament deleted successfully"
        });
    }
}
