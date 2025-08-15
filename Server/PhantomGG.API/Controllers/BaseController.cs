using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PhantomGG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected Guid? CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    protected string? CurrentUserEmail => User.FindFirstValue(ClaimTypes.Email);

    protected string? CurrentUserRole => User.FindFirstValue(ClaimTypes.Role);

    protected bool IsInRole(string role) => User.IsInRole(role);

    protected bool IsAdmin => IsInRole("Admin");

    protected ActionResult Created<T>(string routeName, object routeValues, T resource)
    {
        return CreatedAtRoute(routeName, routeValues, resource);
    }

    protected ActionResult NotFound(string message)
    {
        return NotFound(new { message });
    }

    protected ActionResult BadRequest(string message)
    {
        return BadRequest(new { message });
    }

    protected ActionResult Forbidden(string message = "You don't have permission to access this resource")
    {
        return StatusCode(403, new { message });
    }

    protected ActionResult Conflict(string message)
    {
        return StatusCode(409, new { message });
    }
}
