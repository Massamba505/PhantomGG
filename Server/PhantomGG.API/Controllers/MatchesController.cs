using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController(
    IMatchService matchService,
    IMatchEventService matchEventService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private readonly IMatchService _matchService = matchService;
    private readonly IMatchEventService _matchEventService = matchEventService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Search or list matches
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<MatchDto>>> GetMatches([FromQuery] MatchQuery query)
    {
        var result = await _matchService.SearchAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Get match details
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MatchDto>> GetMatch(Guid id)
    {
        var match = await _matchService.GetByIdAsync(id);
        return Ok(match);
    }

    /// <summary>
    /// Create a match
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MatchDto>> CreateMatch([FromBody] CreateMatchDto createDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var match = await _matchService.CreateAsync(createDto, currentUser.Id);

        return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
    }

    /// <summary>
    /// Update match info
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<MatchDto>> UpdateMatch(Guid id, [FromBody] UpdateMatchDto updateDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var match = await _matchService.UpdateAsync(id, updateDto, currentUser.Id);
        return Ok(match);
    }

    /// <summary>
    /// Delete match
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteMatch(Guid id)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _matchService.DeleteAsync(id, currentUser.Id);
        return NoContent();
    }

    /// <summary>
    /// Submit or update match result
    /// </summary>
    [HttpPatch("{id:guid}/result")]
    [Authorize]
    public async Task<ActionResult<MatchDto>> UpdateMatchResult(Guid id, [FromBody] MatchResultDto resultDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var match = await _matchService.UpdateResultAsync(id, resultDto, currentUser.Id);
        return Ok(match);
    }

    /// <summary>
    /// List events for a match
    /// </summary>
    [HttpGet("{id:guid}/events")]
    public async Task<ActionResult<IEnumerable<MatchEventDto>>> GetMatchEvents(
        Guid id,
        [FromQuery] MatchEventType? type)
    {
        var events = await _matchEventService.GetMatchEventsAsync(id);

        if (type.HasValue)
        {
            events = events.Where(e => e.EventType == type);
        }

        return Ok(events);
    }

    /// <summary>
    /// Add an event to a match
    /// </summary>
    [HttpPost("{id:guid}/events")]
    [Authorize]
    public async Task<ActionResult<MatchEventDto>> CreateMatchEvent(Guid id, [FromBody] CreateMatchEventDto createDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();

        // Ensure match ID consistency
        createDto.MatchId = id;

        var createdEvent = await _matchEventService.CreateMatchEventAsync(createDto, currentUser.Id);

        return CreatedAtAction(
            nameof(GetMatchEvent),
            new { matchId = id, eventId = createdEvent.Id },
            createdEvent);
    }

    /// <summary>
    /// Get an event
    /// </summary>
    [HttpGet("{matchId:guid}/events/{eventId:guid}")]
    public async Task<ActionResult<MatchEventDto>> GetMatchEvent(Guid matchId, Guid eventId)
    {
        var matchEvent = await _matchEventService.GetMatchEventByIdAsync(eventId);
        return Ok(matchEvent);
    }

    /// <summary>
    /// Update an event
    /// </summary>
    [HttpPatch("{matchId:guid}/events/{eventId:guid}")]
    [Authorize]
    public async Task<ActionResult<MatchEventDto>> UpdateMatchEvent(
        Guid matchId,
        Guid eventId,
        [FromBody] UpdateMatchEventDto updateDto)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var updatedEvent = await _matchEventService.UpdateMatchEventAsync(eventId, updateDto, currentUser.Id);
        return Ok(updatedEvent);
    }

    /// <summary>
    /// Delete an event
    /// </summary>
    [HttpDelete("{matchId:guid}/events/{eventId:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteMatchEvent(Guid matchId, Guid eventId)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        await _matchEventService.DeleteMatchEventAsync(eventId, currentUser.Id);
        return NoContent();
    }

    /// <summary>
    /// Get all events for a specific player
    /// </summary>
    [HttpGet("player/{playerId:guid}/events")]
    public async Task<ActionResult<IEnumerable<MatchEventDto>>> GetPlayerEvents(Guid playerId)
    {
        var events = await _matchEventService.GetPlayerEventsAsync(playerId);
        return Ok(events);
    }

    /// <summary>
    /// Get all events for a specific team
    /// </summary>
    [HttpGet("team/{teamId:guid}/events")]
    public async Task<ActionResult<IEnumerable<MatchEventDto>>> GetTeamEvents(Guid teamId)
    {
        var events = await _matchEventService.GetTeamEventsAsync(teamId);
        return Ok(events);
    }
}
