using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.Player;

public class PlayerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public PlayerPosition Position { get; set; }
    public string? PhotoUrl { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}
