namespace PhantomGG.Models.DTOs.Player;

public class PlayerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Position { get; set; }
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}
