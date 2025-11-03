namespace PhantomGG.Models.DTOs.Match;

public class CreateMatchDto
{
    public Guid TournamentId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public DateTime MatchDate { get; set; }
    public string? Venue { get; set; }
}
