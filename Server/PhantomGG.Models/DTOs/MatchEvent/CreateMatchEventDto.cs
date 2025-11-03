using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.MatchEvent;

public class CreateMatchEventDto
{
    public Guid MatchId { get; set; }
    public MatchEventType EventType { get; set; }
    public int Minute { get; set; }
    public Guid TeamId { get; set; }
    public Guid PlayerId { get; set; }
}
