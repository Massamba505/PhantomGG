using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Exceptions;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController(
            IMatchService matchService,
            ICurrentUserService currentUserService) : ControllerBase
{
    private readonly IMatchService _matchService = matchService;
    private readonly ICurrentUserService _currentUserService = currentUserService;


    /// <summary>
    /// Get a specific match by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetMatch(Guid id)
    {
        try
        {
            var match = await _matchService.GetByIdAsync(id);
            return Ok(new ApiResponse
            {
                Success = true,
                Data = match,
                Message = "Match retrieved successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all matches for a specific tournament
    /// </summary>
    [HttpGet("tournament/{tournamentId}")]
    public async Task<ActionResult<ApiResponse>> GetMatchesByTournament(Guid tournamentId)
    {
        var matches = await _matchService.GetByTournamentAsync(tournamentId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = matches,
            Message = "Tournament matches retrieved successfully"
        });
    }

    /// <summary>
    /// Get all matches for a specific team
    /// </summary>
    [HttpGet("team/{teamId}")]
    public async Task<ActionResult<ApiResponse>> GetMatchesByTeam(Guid teamId)
    {
        var matches = await _matchService.GetByTeamAsync(teamId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = matches,
            Message = "Team matches retrieved successfully"
        });
    }

    /// <summary>
    /// Search matches with various filters
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse>> SearchMatches([FromQuery] MatchSearchDto searchDto)
    {
        var matches = await _matchService.SearchAsync(searchDto);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = matches,
            Message = "Match search completed"
        });
    }

    /// <summary>
    /// Create a new match
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateMatch([FromBody] CreateMatchDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var match = await _matchService.CreateAsync(createDto, userId);

            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, new ApiResponse
            {
                Success = true,
                Data = match,
                Message = "Match created successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update an existing match
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateMatch(Guid id, [FromBody] UpdateMatchDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var match = await _matchService.UpdateAsync(id, updateDto, userId);

            return Ok(new ApiResponse
            {
                Success = true,
                Data = match,
                Message = "Match updated successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update match result and score
    /// </summary>
    [HttpPut("{id}/result")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateMatchResult(Guid id, [FromBody] MatchResultDto resultDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var match = await _matchService.UpdateResultAsync(id, resultDto, userId);

            return Ok(new ApiResponse
            {
                Success = true,
                Data = match,
                Message = "Match result updated successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete a match
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteMatch(Guid id)
    {
        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            await _matchService.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }


    /// <summary>
    /// Generate round-robin fixtures for a tournament
    /// </summary>
    [HttpPost("generate-fixtures")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GenerateFixtures([FromBody] GenerateFixturesDto generateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var matches = await _matchService.GenerateRoundRobinFixturesAsync(generateDto, userId);

            return Ok(new ApiResponse
            {
                Success = true,
                Data = matches,
                Message = "Fixtures generated successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (ForbiddenException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get fixture generation status for a tournament
    /// </summary>
    [HttpGet("fixture-status/{tournamentId}")]
    public async Task<ActionResult<ApiResponse>> GetFixtureGenerationStatus(Guid tournamentId)
    {
        try
        {
            var status = await _matchService.GetFixtureGenerationStatusAsync(tournamentId);
            return Ok(new ApiResponse
            {
                Success = true,
                Data = status,
                Message = "Fixture status retrieved successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }



    /// <summary>
    /// Get all events for a specific match
    /// </summary>
    [HttpGet("{matchId}/events")]
    public async Task<ActionResult<ApiResponse>> GetMatchEvents(Guid matchId)
    {
        var events = await _matchService.GetMatchEventsAsync(matchId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = events,
            Message = "Match events retrieved successfully"
        });
    }

    /// <summary>
    /// Get a specific match event by ID
    /// </summary>
    [HttpGet("events/{eventId}")]
    public async Task<ActionResult<ApiResponse>> GetMatchEvent(Guid eventId)
    {
        try
        {
            var matchEvent = await _matchService.GetMatchEventByIdAsync(eventId);
            return Ok(new ApiResponse
            {
                Success = true,
                Data = matchEvent,
                Message = "Match event retrieved successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all events for a specific player
    /// </summary>
    [HttpGet("player/{playerId}/events")]
    public async Task<ActionResult<ApiResponse>> GetPlayerEvents(Guid playerId)
    {
        var events = await _matchService.GetPlayerEventsAsync(playerId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = events,
            Message = "Player events retrieved successfully"
        });
    }

    /// <summary>
    /// Get all events for a specific team
    /// </summary>
    [HttpGet("team/{teamId}/events")]
    public async Task<ActionResult<ApiResponse>> GetTeamEvents(Guid teamId)
    {
        var events = await _matchService.GetTeamEventsAsync(teamId);
        return Ok(new ApiResponse
        {
            Success = true,
            Data = events,
            Message = "Team events retrieved successfully"
        });
    }

    /// <summary>
    /// Create a new match event
    /// </summary>
    [HttpPost("events")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateMatchEvent([FromBody] CreateMatchEventDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var createdEvent = await _matchService.CreateMatchEventAsync(createDto, userId);

            return CreatedAtAction(
                nameof(GetMatchEvent),
                new { eventId = createdEvent.Id },
                new ApiResponse
                {
                    Success = true,
                    Data = createdEvent,
                    Message = "Match event created successfully"
                });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update an existing match event
    /// </summary>
    [HttpPut("events/{eventId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateMatchEvent(Guid eventId, [FromBody] UpdateMatchEventDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var updatedEvent = await _matchService.UpdateMatchEventAsync(eventId, updateDto, userId);

            return Ok(new ApiResponse
            {
                Success = true,
                Data = updatedEvent,
                Message = "Match event updated successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete a match event
    /// </summary>
    [HttpDelete("events/{eventId}")]
    [Authorize]
    public async Task<ActionResult> DeleteMatchEvent(Guid eventId)
    {
        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            await _matchService.DeleteMatchEventAsync(eventId, userId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

}
