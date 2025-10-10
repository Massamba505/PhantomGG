using PhantomGG.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Match;

public class UpdateMatchDto
{
    [Required]
    public DateTime MatchDate { get; set; }

    [StringLength(200)]
    public string? Venue { get; set; }

    [Required]
    public MatchStatus Status { get; set; }
}
