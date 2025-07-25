using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;

namespace PhantomGG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TournamentsController(PhantomGGContext context) : ControllerBase
{
    private readonly PhantomGGContext _context = context;

    /// <summary>
    /// Counts the number of tournaments in the database.
    /// </summary>
    /// <returns>
    /// Returns a success message with the count.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> CountTournamentsAsync()
    {
        var tournamentNum = await _context.Tournaments.CountAsync();
        return Ok(new { success = true, message = $"There are {tournamentNum} tournaments" });
    }
}
