using Microsoft.AspNetCore.Mvc;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackgroundJobsController(ITournamentBackgroundJobService backgroundJobService) : ControllerBase
{
    private readonly ITournamentBackgroundJobService _backgroundJobService = backgroundJobService;

    /// <summary>
    /// Manually trigger tournament status update (for testing purposes)
    /// </summary>
    [HttpPost("update-tournament-statuses")]
    public async Task<IActionResult> UpdateTournamentStatuses()
    {
        try
        {
            await _backgroundJobService.UpdateTournamentStatusesAsync();
            return Ok(new { Message = "Tournament status update completed successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error updating tournament statuses", Error = ex.Message });
        }
    }
}
