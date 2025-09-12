using PhantomGG.API.Common;

namespace PhantomGG.API.DTOs.Tournament;

public class TournamentSearchDto
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public string? Location { get; set; }
    public string? FormatId { get; set; }
    public decimal? MinPrizePool { get; set; }
    public decimal? MaxPrizePool { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public bool? IsPublic { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
