using System.ComponentModel.DataAnnotations;
using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.MatchEvent;

public class UpdateMatchEventDto
{
    public MatchEventType? EventType { get; set; }
    public int? Minute { get; set; }
    public Guid? PlayerId { get; set; }
}
