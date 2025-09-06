using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.MatchEvent;

public class CreateMatchEventDto
{
    [Required]
    public Guid MatchId { get; set; }

    [Required]
    [StringLength(20)]
    public string EventType { get; set; } = null!;

    [Required]
    [Range(0, 120)]
    public int Minute { get; set; }

    [Required]
    public Guid TeamId { get; set; }

    [StringLength(100)]
    public string? PlayerName { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
