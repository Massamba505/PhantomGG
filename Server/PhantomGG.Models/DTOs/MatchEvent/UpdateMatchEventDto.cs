using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.MatchEvent;

public class UpdateMatchEventDto
{
    [Required]
    [Range(0, 120)]
    public int Minute { get; set; }

    [StringLength(100)]
    public string? PlayerName { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
