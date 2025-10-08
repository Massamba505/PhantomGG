namespace PhantomGG.Models.DTOs.Match;

public class MatchQuery
{
    public string? Q { get; set; }
    public Guid? TournamentId { get; set; }
    public Guid? TeamId { get; set; }
    public string? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
