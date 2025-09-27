namespace PhantomGG.Models.DTOs.TournamentStanding;

public class TournamentStandingDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? TeamLogo { get; set; }
    public int MatchesPlayed { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points { get; set; }
    public int? Position { get; set; }
}