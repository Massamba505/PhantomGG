using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController(
    ITournamentService tournamentService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITournamentService _tournamentService = tournamentService;
    private readonly ICurrentUserService _currentUserService = currentUserService;


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
        var user = _currentUserService.GetCurrentUser();
        var tournaments = await _tournamentService.GetByOrganizerAsync(user.Id);
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

    [HttpPut("{id:guid}")]
    [Authorize]
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

    [HttpDelete("{id:guid}")]
    [Authorize]
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
}
