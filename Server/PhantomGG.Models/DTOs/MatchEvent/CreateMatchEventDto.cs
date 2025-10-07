using System.ComponentModel.DataAnnotations;
using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.MatchEvent;

public class CreateMatchEventDto
{
    [Required]
    public Guid MatchId { get; set; }

    [Required]
    public MatchEventType EventType { get; set; }

    [Required]
    [Range(0, 120)]
    public int Minute { get; set; }

    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public Guid PlayerId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
