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

    protected IEnumerable<string> CurrentUserRoles => User.FindAll(ClaimTypes.Role).Select(c => c.Value);

    protected bool IsInRole(string role) => User.IsInRole(role);

    protected IActionResult Success<T>(T data, string message = "Success")
    {
        return Ok(new
        {
            success = true,
            message,
            data
        });
    }

    protected IActionResult Success(string message = "Success")
    {
        return Ok(new
        {
            success = true,
            message
        });
    }

    protected IActionResult Error(string message, int statusCode = 400)
    {
        return StatusCode(statusCode, new
        {
            success = false,
            message
        });
    }

    protected IActionResult ValidationError(string message = "Validation failed")
    {
        return BadRequest(new
        {
            success = false,
            message,
            errors = ModelState.Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                )
        });
    }
}
