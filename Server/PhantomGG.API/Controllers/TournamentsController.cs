using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.TournamentStanding;
using PhantomGG.Service.Domain.Tournaments.Interfaces;
using PhantomGG.Service.Domain.Matches.Interfaces;
using PhantomGG.Service.Auth.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController(
    ITournamentService tournamentService,
    ITournamentTeamService tournamentTeamService,
    IMatchService matchService,
    ITournamentStandingService tournamentStandingService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ITournamentService _tournamentService = tournamentService;
    private readonly ITournamentTeamService _tournamentTeamService = tournamentTeamService;
    private readonly IMatchService _matchService = matchService;
    private readonly ITournamentStandingService _tournamentStandingService = tournamentStandingService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// List or search tournaments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<TournamentDto>>> GetTournaments([FromQuery] TournamentQuery query)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        Guid? userId = currentUser?.Role == UserRoles.Organizer ? currentUser?.Id : null;

        var tournaments = await _tournamentService.SearchAsync(query, userId);
        return Ok(tournaments);
    }

    /// <summary>
    /// Get tournament details
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TournamentDto>> GetTournament(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.GetByIdAsync(id, currentUser?.Id);
        return Ok(tournament);
    }

    /// <summary>
    /// Create a new tournament
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<TournamentDto>> CreateTournament([FromForm] CreateTournamentDto createDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.CreateAsync(createDto, currentUser.Id);

        return CreatedAtAction(
            nameof(GetTournament),
            new { id = tournament.Id },
            tournament);
    }

    /// <summary>
    /// Update tournament details
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<TournamentDto>> UpdateTournament(Guid id, [FromForm] UpdateTournamentDto updateDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var tournament = await _tournamentService.UpdateAsync(id, updateDto, currentUser.Id);
        return Ok(tournament);
    }

    /// <summary>
    /// Delete tournament
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult> DeleteTournament(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentService.DeleteAsync(id, currentUser.Id);
        return NoContent();
    }

    /// <summary>
    /// Get tournament teams with status filter
    /// </summary>
    [HttpGet("{id:guid}/teams")]
    public async Task<ActionResult<IEnumerable<TournamentTeamDto>>> GetTournamentTeams(
        Guid id,
        [FromQuery] TeamRegistrationStatus status = TeamRegistrationStatus.Approved)
    {
        var tournamentTeams = await _tournamentTeamService.GetTeamsAsync(id, status);
        return Ok(tournamentTeams);
    }

    /// <summary>
    /// Register a team to a tournament
    /// </summary>
    [HttpPost("{tournamentId:guid}/teams/{teamId:guid}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> RegisterTeam(Guid tournamentId, Guid teamId)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentTeamService.RegisterTeamAsync(tournamentId, teamId, currentUser.Id);

        return Created($"/tournaments/{tournamentId}/teams", null);
    }

    /// <summary>
    /// Manage team's participation in a tournament (approve, reject, withdraw)
    /// </summary>
    [HttpPatch("{tournamentId:guid}/teams/{teamId:guid}")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult> ManageTeamParticipation(
        Guid tournamentId,
        Guid teamId,
        [FromBody] TeamManagementRequest request)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _tournamentTeamService.ManageTeamAsync(tournamentId, teamId, request.Action, currentUser.Id);

        return Ok();
    }

    /// <summary>
    /// Get tournament matches
    /// </summary>
    [HttpGet("{id:guid}/matches")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetTournamentMatches(
        Guid id,
        [FromQuery] MatchStatus? status)
    {
        var matches = await _matchService.GetByTournamentAndStatusAsync(id, status);
        return Ok(matches);
    }

    /// <summary>
    /// Get tournament standings
    /// </summary>
    [HttpGet("{id:guid}/standings")]
    public async Task<ActionResult<IEnumerable<TournamentStandingDto>>> GetTournamentStandings(Guid id)
    {
        var standings = await _tournamentStandingService.GetTournamentStandingsAsync(id);
        return Ok(standings);
    }

    /// <summary>
    /// Get player goal standings for tournament
    /// </summary>
    [HttpGet("{id:guid}/standings/goals")]
    public async Task<ActionResult<IEnumerable<PlayerGoalStandingDto>>> GetPlayerGoalStandings(Guid id)
    {
        var standings = await _tournamentStandingService.GetPlayerGoalStandingsAsync(id);
        return Ok(standings);
    }

    /// <summary>
    /// Get player assist standings for tournament
    /// </summary>
    [HttpGet("{id:guid}/standings/assists")]
    public async Task<ActionResult<IEnumerable<PlayerAssistStandingDto>>> GetPlayerAssistStandings(Guid id)
    {
        var standings = await _tournamentStandingService.GetPlayerAssistStandingsAsync(id);
        return Ok(standings);
    }

    /// <summary>
    /// Generate tournament fixtures
    /// </summary>
    [HttpPost("{id:guid}/fixtures")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult> GenerateFixtures(Guid id, [FromBody] GenerateFixturesRequest request)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _matchService.CreateTournamentBracketAsync(id, currentUser!.Id);

        return Accepted();
    }
}
