using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Exceptions;

namespace PhantomGG.API.Controllers;

/// <summary>
/// Controller for managing matches and fixtures in tournaments
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;
    private readonly ICurrentUserService _currentUserService;

    public MatchesController(IMatchService matchService, ICurrentUserService currentUserService)
    {
        _matchService = matchService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get all matches in the system
    /// </summary>
    /// <returns>A list of all matches</returns>
    /// <response code="200">Returns the list of matches</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MatchDto>), 200)]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetAllMatches()
    {
        var matches = await _matchService.GetAllAsync();
        return Ok(matches);
    }

    /// <summary>
    /// Get a specific match by ID
    /// </summary>
    /// <param name="id">The match ID</param>
    /// <returns>The match details</returns>
    /// <response code="200">Returns the match</response>
    /// <response code="404">If the match is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MatchDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MatchDto>> GetMatch(Guid id)
    {
        try
        {
            var match = await _matchService.GetByIdAsync(id);
            return Ok(match);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get all matches for a specific tournament
    /// </summary>
    /// <param name="tournamentId">The tournament ID</param>
    /// <returns>A list of matches for the tournament</returns>
    /// <response code="200">Returns the list of matches</response>
    [HttpGet("tournament/{tournamentId}")]
    [ProducesResponseType(typeof(IEnumerable<MatchDto>), 200)]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByTournament(Guid tournamentId)
    {
        var matches = await _matchService.GetByTournamentAsync(tournamentId);
        return Ok(matches);
    }

    /// <summary>
    /// Get all matches for a specific team
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <returns>A list of matches for the team</returns>
    /// <response code="200">Returns the list of matches</response>
    [HttpGet("team/{teamId}")]
    [ProducesResponseType(typeof(IEnumerable<MatchDto>), 200)]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByTeam(Guid teamId)
    {
        var matches = await _matchService.GetByTeamAsync(teamId);
        return Ok(matches);
    }

    /// <summary>
    /// Search matches with various filters
    /// </summary>
    /// <param name="searchDto">Search criteria</param>
    /// <returns>A list of matches matching the search criteria</returns>
    /// <response code="200">Returns the filtered list of matches</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<MatchDto>), 200)]
    public async Task<ActionResult<IEnumerable<MatchDto>>> SearchMatches([FromQuery] MatchSearchDto searchDto)
    {
        var matches = await _matchService.SearchAsync(searchDto);
        return Ok(matches);
    }

    /// <summary>
    /// Create a new match
    /// </summary>
    /// <param name="createDto">Match creation data</param>
    /// <returns>The created match</returns>
    /// <response code="201">Returns the newly created match</response>
    /// <response code="400">If the match data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to create matches</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(MatchDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<MatchDto>> CreateMatch([FromBody] CreateMatchDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var match = await _matchService.CreateAsync(createDto, userId);
            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing match
    /// </summary>
    /// <param name="id">The match ID</param>
    /// <param name="updateDto">Match update data</param>
    /// <returns>The updated match</returns>
    /// <response code="200">Returns the updated match</response>
    /// <response code="400">If the match data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to update this match</response>
    /// <response code="404">If the match is not found</response>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(MatchDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MatchDto>> UpdateMatch(Guid id, [FromBody] UpdateMatchDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var match = await _matchService.UpdateAsync(id, updateDto, userId);
            return Ok(match);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (UnauthorizedException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Update match result and score
    /// </summary>
    /// <param name="id">The match ID</param>
    /// <param name="resultDto">Match result data</param>
    /// <returns>The updated match with results</returns>
    /// <response code="200">Returns the updated match with results</response>
    /// <response code="400">If the result data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to update this match result</response>
    /// <response code="404">If the match is not found</response>
    [HttpPut("{id}/result")]
    [Authorize]
    [ProducesResponseType(typeof(MatchDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MatchDto>> UpdateMatchResult(Guid id, [FromBody] MatchResultDto resultDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var match = await _matchService.UpdateResultAsync(id, resultDto, userId);
            return Ok(match);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (UnauthorizedException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Delete a match
    /// </summary>
    /// <param name="id">The match ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Match deleted successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to delete this match</response>
    /// <response code="404">If the match is not found</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteMatch(Guid id)
    {
        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            await _matchService.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (UnauthorizedException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Generate round-robin fixtures for a tournament
    /// </summary>
    /// <param name="generateDto">Fixture generation parameters</param>
    /// <returns>The generated matches</returns>
    /// <response code="200">Returns the generated fixtures</response>
    /// <response code="400">If the generation parameters are invalid or conditions not met</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to generate fixtures</response>
    [HttpPost("generate-fixtures")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<MatchDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GenerateFixtures([FromBody] GenerateFixturesDto generateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var matches = await _matchService.GenerateRoundRobinFixturesAsync(generateDto, userId);
            return Ok(matches);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Automatically generate fixtures based on tournament format (Round Robin or Single Elimination)
    /// </summary>
    /// <param name="generateDto">Auto-generation parameters including tournament format</param>
    /// <returns>The generated matches with success information</returns>
    /// <response code="200">Returns the generated fixtures with success details</response>
    /// <response code="400">If the generation parameters are invalid or conditions not met</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to generate fixtures</response>
    [HttpPost("auto-generate-fixtures")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<MatchDto>>> AutoGenerateFixtures([FromBody] AutoGenerateFixturesDto generateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _currentUserService.GetCurrentUser().Id;
            var matches = await _matchService.AutoGenerateFixturesAsync(generateDto, userId);
            return Ok(new
            {
                success = true,
                message = "Fixtures generated successfully",
                matchCount = matches.Count(),
                matches = matches
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            return StatusCode(403, new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get fixture generation status for a tournament
    /// </summary>
    /// <param name="tournamentId">The tournament ID</param>
    /// <returns>Fixture generation status information</returns>
    /// <response code="200">Returns the fixture generation status</response>
    /// <response code="404">If the tournament is not found</response>
    [HttpGet("fixture-status/{tournamentId}")]
    [ProducesResponseType(typeof(FixtureGenerationStatusDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<FixtureGenerationStatusDto>> GetFixtureGenerationStatus(Guid tournamentId)
    {
        try
        {
            var status = await _matchService.GetFixtureGenerationStatusAsync(tournamentId);
            return Ok(status);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }
}
