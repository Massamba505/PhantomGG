using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Match;

public class GenerateFixturesDto
{
    [Required]
    public Guid TournamentId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public int DaysBetweenMatches { get; set; } = 7;

    public string? DefaultVenue { get; set; }

    public bool IncludeReturnMatches { get; set; } = true;
}
