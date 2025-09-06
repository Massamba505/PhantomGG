using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Match;

public class UpdateMatchDto
{
    [Required]
    public DateTime MatchDate { get; set; }

    [StringLength(200)]
    public string? Venue { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Scheduled";
}
