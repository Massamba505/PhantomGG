using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Match;

public class CreateMatchDto
{
    [Required]
    public Guid TournamentId { get; set; }

    [Required]
    public Guid HomeTeamId { get; set; }

    [Required]
    public Guid AwayTeamId { get; set; }

    [Required]
    public DateTime MatchDate { get; set; }

    [StringLength(200)]
    public string? Venue { get; set; }
}
