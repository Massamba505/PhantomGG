namespace PhantomGG.API.DTOs.TournamentStanding;

public class PlayerAssistStandingDto
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? TeamLogo { get; set; }
    public string? PlayerPhoto { get; set; }
    public int Assists { get; set; }
    public int MatchesPlayed { get; set; }
    public decimal AssistsPerMatch => MatchesPlayed > 0 ? (decimal)Assists / MatchesPlayed : 0;
    public int? Position { get; set; }
}
