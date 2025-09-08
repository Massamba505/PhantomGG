namespace PhantomGG.API.DTOs.TournamentStanding;

public class PlayerGoalStandingDto
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? TeamLogo { get; set; }
    public string? PlayerPhoto { get; set; }
    public int Goals { get; set; }
    public int MatchesPlayed { get; set; }
    public int? Position { get; set; }
}
