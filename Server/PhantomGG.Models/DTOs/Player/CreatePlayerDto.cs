using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.Player;

public class CreatePlayerDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public PlayerPosition Position { get; set; }
    public string? Email { get; set; }
    public IFormFile? PhotoUrl { get; set; }
    public Guid TeamId { get; set; }
}
