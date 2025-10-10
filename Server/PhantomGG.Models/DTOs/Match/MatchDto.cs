using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.Match;

public class MatchDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = null!;
    public Guid HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = null!;
    public string? HomeTeamLogo { get; set; }
    public Guid AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = null!;
    public string? AwayTeamLogo { get; set; }
    public DateTime MatchDate { get; set; }
    public string? Venue { get; set; }
    public MatchStatus Status { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
}
