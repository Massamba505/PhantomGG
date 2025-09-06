namespace PhantomGG.API.DTOs.MatchEvent;

public class MatchEventDto
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public string EventType { get; set; } = null!;
    public int Minute { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public string? PlayerName { get; set; }
    public string? Description { get; set; }
}
