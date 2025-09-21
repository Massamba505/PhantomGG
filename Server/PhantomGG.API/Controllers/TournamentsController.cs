using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.Entities;
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
    /// Get all tournaments with search and filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetTournaments([FromQuery] TournamentSearchDto searchDto)
    {
        var tournaments = await _tournamentService.SearchAsync(searchDto);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Tournament search completed"
        });
    }

    /// <summary>
    /// Get tournament details by ID
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
    /// Get tournament teams
    /// </summary>
    [HttpGet("{id:guid}/teams")]
    public async Task<ActionResult<ApiResponse>> GetTournamentTeams(Guid id)
    {
        var teams = await _tournamentService.GetTournamentTeamsAsync(id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Tournament teams retrieved successfully"
        });
    }

    /// <summary>
    /// Get organizer's tournaments
    /// </summary>
    [HttpGet("my-tournaments")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetMyTournaments([FromQuery] TournamentSearchDto searchDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var tournaments = await _tournamentService.GetMyTournamentsAsync(searchDto, currentUser.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Your tournaments retrieved successfully"
        });
    }

    /// <summary>
    /// Create new tournament
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> CreateTournament([FromBody] CreateTournamentDto createDto)
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
    public async Task<ActionResult<ApiResponse>> UpdateTournament(Guid id, [FromBody] UpdateTournamentDto updateDto)
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
    /// Upload tournament image
    /// </summary>
    [HttpPost("{id:guid}/image/banner")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> UploadTournamentBannerImage(Guid id, IFormFile file)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.GetByIdAsync(id);

        if (tournament == null)
            return NotFound(new ApiResponse { Success = false, Message = "Tournament not found" });

        if (tournament.OrganizerId != currentUser.Id)
            return Forbid();

        var tournamentEntity = new Tournament
        {
            Id = tournament.Id,
            Name = tournament.Name,
            OrganizerId = tournament.OrganizerId,
            BannerUrl = tournament.BannerUrl,
            LogoUrl = tournament.LogoUrl
        };

        var imageUrl = await _tournamentService.UploadImageAsync(tournamentEntity, file);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { imageUrl },
            Message = "Tournament image uploaded successfully"
        });
    }

    /// <summary>
    /// Upload tournament image
    /// </summary>
    [HttpPost("{id:guid}/image/logo")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> UploadTournamentLogoImage(Guid id, IFormFile file)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.GetByIdAsync(id);

        if (tournament == null)
            return NotFound(new ApiResponse { Success = false, Message = "Tournament not found" });

        if (tournament.OrganizerId != currentUser.Id)
            return Forbid();

        var tournamentEntity = new Tournament
        {
            Id = tournament.Id,
            Name = tournament.Name,
            OrganizerId = tournament.OrganizerId,
            BannerUrl = tournament.BannerUrl,
            LogoUrl = tournament.LogoUrl
        };

        var imageUrl = await _tournamentService.UploadImageAsync(tournamentEntity, file);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { imageUrl },
            Message = "Tournament image uploaded successfully"
        });
    }

    /// <summary>
    /// Register team for tournament
    /// </summary>
    [HttpPost("{id:guid}/register")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RegisterForTournament(Guid id, [FromBody] JoinTournamentDto registrationDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.RegisterForTournamentAsync(id, registrationDto, currentUser.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team registered for tournament successfully"
        });
    }

    /// <summary>
    /// Withdraw team from tournament
    /// </summary>
    [HttpPost("{id:guid}/withdraw")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> WithdrawFromTournament(Guid id, [FromBody] LeaveTournamentDto withdrawDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.WithdrawFromTournamentAsync(id, withdrawDto, currentUser.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team withdrawn from tournament successfully"
        });
    }

    /// <summary>
    /// Get teams pending approval for tournament
    /// </summary>
    [HttpGet("{id:guid}/teams/pending")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> GetPendingTeams(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var pendingTeams = await _tournamentService.GetPendingTeamsAsync(id, currentUser.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Data = pendingTeams,
            Message = "Pending teams retrieved successfully"
        });
    }

    /// <summary>
    /// Approve team registration
    /// </summary>
    [HttpPost("{tournamentId:guid}/teams/{teamId:guid}/approve")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> ApproveTeam(Guid tournamentId, Guid teamId)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.ApproveTeamAsync(tournamentId, teamId, currentUser.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team approved successfully"
        });
    }

    /// <summary>
    /// Reject team registration
    /// </summary>
    [HttpPost("{tournamentId:guid}/teams/{teamId:guid}/reject")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> RejectTeam(Guid tournamentId, Guid teamId)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.RejectTeamAsync(tournamentId, teamId, currentUser.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team rejected successfully"
        });
    }

    /// <summary>
    /// Remove team from tournament
    /// </summary>
    [HttpDelete("{tournamentId:guid}/teams/{teamId:guid}")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<ApiResponse>> RemoveTeam(Guid tournamentId, Guid teamId)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.WithdrawFromTournamentAsync(tournamentId, new LeaveTournamentDto { TeamId = teamId }, currentUser.Id);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Team removed from tournament successfully"
        });
    }
}
