using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Match;

public class MatchResultDto
{
    [Required]
    public int HomeScore { get; set; }

    [Required]
    public int AwayScore { get; set; }
}
