using Microsoft.AspNetCore.Mvc;

namespace PhantomGG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            message = "PhantomGG API is running successfully"
        });
    }
}
