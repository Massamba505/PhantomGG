namespace PhantomGG.Models.DTOs.Tournament;

public class TournamentStatisticsDto
{
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public int TotalTeams { get; set; }
    public int ApprovedTeams { get; set; }
    public int PendingTeams { get; set; }
    public int TotalPlayers { get; set; }
    public int TotalMatches { get; set; }
    public int CompletedMatches { get; set; }
    public int ScheduledMatches { get; set; }
    public int TotalGoals { get; set; }
    public int TotalYellowCards { get; set; }
    public int TotalRedCards { get; set; }
    public decimal AverageGoalsPerMatch { get; set; }
    public string TopScorer { get; set; } = string.Empty;
    public int TopScorerGoals { get; set; }
    public string TopAssist { get; set; } = string.Empty;
    public int TopAssistCount { get; set; }
    public string MostDisciplinedTeam { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}
