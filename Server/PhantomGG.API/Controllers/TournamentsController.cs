using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController(
    ITournamentService tournamentService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITournamentService _tournamentService = tournamentService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// tournament search
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetTournaments([FromQuery] TournamentSearchDto searchDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        Guid? userId = currentUser.Role == UserRoles.Organizer.ToString()? currentUser?.Id : null;

        var tournaments = await _tournamentService.SearchAsync(searchDto, userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Tournament search completed"
        });
    }

    /// <summary>
    /// tournament details
    /// </summary>
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

    /// <summary>
    /// tournament teams with status filter
    /// </summary>
    [HttpGet("{id:guid}/teams")]
    public async Task<ActionResult<ApiResponse>> GetTournamentTeams(Guid id, [FromQuery] TeamRegistrationStatus status = TeamRegistrationStatus.Approved)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        Guid? userId = currentUser.Role == UserRoles.Organizer.ToString() ? currentUser?.Id : null;

        var tournamentTeams = await _tournamentService.GetTournamentTeamsAsync(id, userId, status);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournamentTeams,
            Message = "Tournament teams retrieved successfully"
        });
    }

    /// <summary>
    /// tournament matches
    /// </summary>
    [HttpGet("{id:guid}/matches")]
    public async Task<ActionResult<ApiResponse>> GetTournamentMatches(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        Guid? userId = currentUser?.Id;

        var matches = await _tournamentService.GetTournamentMatchesAsync(id, userId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = matches,
            Message = "Tournament matches retrieved successfully"
        });
    }

    /// <summary>
    /// Tournament standings
    /// </summary>
    [HttpGet("{id:guid}/standings")]
    public async Task<ActionResult<ApiResponse>> GetTournamentStandings(Guid id)
    {
        var standings = await _tournamentService.GetTournamentStandingsAsync(id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = standings,
            Message = "Tournament standings retrieved successfully"
        });
    }

    /// <summary>
    /// Create new tournament
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> CreateTournament([FromForm] CreateTournamentDto createDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.CreateAsync(createDto, currentUser.Id);
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
    /// Update tournament details
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> UpdateTournament(Guid id, [FromForm] UpdateTournamentDto updateDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.UpdateAsync(id, updateDto, currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournament,
            Message = "Tournament updated successfully"
        });
    }

    /// <summary>
    /// Delete tournament
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> DeleteTournament(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.DeleteAsync(id, currentUser.Id);
        return NoContent();
    }

    /// <summary>
    /// Tournament team management endpoint
    /// Handles: register, withdraw, approve, reject actions
    /// </summary>
    [HttpPut("{tournamentId:guid}/teams/{teamId:guid?}")]
    public async Task<ActionResult<ApiResponse>> ManageTeam(Guid tournamentId, Guid? teamId, [FromBody] TeamActionDto actionDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.ManageTeamAsync(tournamentId, teamId, actionDto, currentUser.Id);

        var actionMessages = new Dictionary<TeamAction, string>
        {
            { TeamAction.Register, "Team registered for tournament successfully" },
            { TeamAction.Withdraw, "Team withdrawn from tournament successfully" },
            { TeamAction.Approve, "Team approved successfully" },
            { TeamAction.Reject, "Team rejected successfully" }
        };

        return Ok(new ApiResponse
        {
            Success = true,
            Message = actionMessages.GetValueOrDefault(actionDto.Action, "Team action completed successfully")
        });
    }

    /// <summary>
    /// Create tournament bracket
    /// </summary>
    [HttpPost("{id:guid}/bracket")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> CreateTournamentBracket(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.CreateTournamentBracketAsync(id, currentUser.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Tournament bracket created successfully"
        });
    }
}
