namespace PhantomGG.Models.DTOs.Match;

public class MatchSearchDto
{
    public string? SearchTerm { get; set; }
    public Guid? TournamentId { get; set; }
    public string? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Venue { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
