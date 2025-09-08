using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.API.Common;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController(
    ITournamentService tournamentService,
    ITeamService teamService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITournamentService _tournamentService = tournamentService;
    private readonly ITeamService _teamService = teamService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Get all tournaments with optional filtering and pagination
    /// </summary>
    /// <param name="pageNumber">Page number for pagination</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="status">Filter by tournament status</param>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetTournaments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null)
    {
        var tournaments = await _tournamentService.GetAllAsync();

        // Filter by status if provided
        if (!string.IsNullOrEmpty(status))
        {
            tournaments = tournaments.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        var pagedTournaments = tournaments
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { tournaments = pagedTournaments, totalCount = tournaments.Count() },
            Message = "Tournaments retrieved successfully"
        });
    }

    /// <summary>
    /// Get a specific tournament by ID
    /// </summary>
    /// <param name="id">The tournament ID</param>
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
    /// Search tournaments with various filters
    /// </summary>
    /// <param name="searchTerm">Search term for tournament name or description</param>
    /// <param name="startDate">Filter by start date (tournaments starting after this date)</param>
    /// <param name="endDate">Filter by end date (tournaments ending before this date)</param>
    /// <param name="status">Filter by tournament status</param>
    /// <param name="pageNumber">Page number for pagination</param>
    /// <param name="pageSize">Number of items per page</param>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse>> SearchTournaments(
        [FromQuery] string? searchTerm = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var searchDto = new TournamentSearchDto
        {
            SearchTerm = searchTerm,
            StartDate = startDate,
            EndDate = endDate,
            Status = string.IsNullOrEmpty(status) ? null : Enum.Parse<TournamentStatus>(status, true),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var tournaments = await _tournamentService.SearchAsync(searchDto);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = tournaments,
            Message = "Search completed successfully"
        });
    }

    /// <summary>
    /// Get tournaments organized by the current user
    /// </summary>
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

    /// <summary>
    /// Create a new tournament
    /// </summary>
    /// <param name="createDto">Tournament creation data</param>
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

    /// <summary>
    /// Update an existing tournament
    /// </summary>
    /// <param name="id">The tournament ID</param>
    /// <param name="updateDto">Tournament update data</param>
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

    [HttpPost("{id:guid}/banner")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UploadTournamentBanner(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "No file provided"
            });
        }

        var user = _currentUserService.GetCurrentUser();
        var bannerUrl = await _tournamentService.UploadTournamentBannerAsync(id, file, user.Id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = new { bannerUrl },
            Message = "Tournament banner uploaded successfully"
        });
    }

    /// <summary>
    /// Get active/upcoming tournaments
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse>> GetActiveTournaments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var searchDto = new TournamentSearchDto
        {
            Status = TournamentStatus.InProgress,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var activeTournaments = await _tournamentService.SearchAsync(searchDto);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = activeTournaments,
            Message = "Active tournaments retrieved successfully"
        });
    }

    /// <summary>
    /// Get tournament statistics
    /// </summary>
    [HttpGet("{id:guid}/statistics")]
    public async Task<ActionResult<ApiResponse>> GetTournamentStatistics(Guid id)
    {
        var tournament = await _tournamentService.GetByIdAsync(id);

        // Basic statistics for MVP
        var statistics = new
        {
            tournament.Id,
            tournament.Name,
            tournament.Status,
            tournament.MaxTeams,
            tournament.PrizePool,
            tournament.TeamCount,
            tournament.MatchCount,
            tournament.CompletedMatches,
            tournament.RegistrationStartDate,
            tournament.RegistrationDeadline,
            tournament.StartDate,
            tournament.CreatedAt
        };

        return Ok(new ApiResponse
        {
            Success = true,
            Data = statistics,
            Message = "Tournament statistics retrieved successfully"
        });
    }

    /// <summary>
    /// Get tournament teams
    /// </summary>
    [HttpGet("{id:guid}/teams")]
    public async Task<ActionResult<ApiResponse>> GetTournamentTeams(Guid id)
    {
        var teams = await _teamService.GetByTournamentAsync(id);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = teams,
            Message = "Tournament teams retrieved successfully"
        });
    }
}
