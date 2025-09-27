using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Match;

public class AutoGenerateFixturesDto
{
    [Required]
    public Guid TournamentId { get; set; }

    [Required]
    [StringLength(50)]
    public string TournamentFormat { get; set; } = null!; // "RoundRobin" or "SingleElimination"

    [Required]
    public DateTime StartDate { get; set; }

    public int DaysBetweenRounds { get; set; } = 7;

    public string? DefaultVenue { get; set; }

    public bool IncludeReturnMatches { get; set; } = false; // For round robin

    public bool AutoAdvanceTeams { get; set; } = true; // For single elimination

    public DateTime? TimeOfDay { get; set; } // Default kick-off time for matches
}

public class FixtureGenerationStatusDto
{
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = null!;
    public DateTime? FixturesGeneratedAt { get; set; }
    public string? FixturesGeneratedBy { get; set; }
    public int RegisteredTeams { get; set; }
    public int RequiredTeams { get; set; }
    public int MaxTeams { get; set; }
    public string Status { get; set; } = null!;
    public bool CanGenerateFixtures { get; set; }
    public string? Message { get; set; }
}
