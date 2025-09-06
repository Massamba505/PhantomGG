namespace PhantomGG.API.DTOs.Tournament;

public class TournamentSearchDto
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public string? GameType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
