using PhantomGG.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Match;

public class MatchResultDto
{
    [Required]
    public int HomeScore { get; set; }

    [Required]
    public int AwayScore { get; set; }
    public MatchStatus Status { get; set; }
}
