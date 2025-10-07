using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.MatchEvent;

public class MatchEventDto
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public string EventType { get; set; } = String.Empty;
    public int Minute { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = null!;
}
