using PhantomGG.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Match;

public class MatchResultDto
{
    /// <summary>
    /// Match status - scores are automatically calculated from goal events
    /// </summary>
    [Required]
    public MatchStatus Status { get; set; }
}
