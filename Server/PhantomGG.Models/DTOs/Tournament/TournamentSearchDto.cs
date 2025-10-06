using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.Tournament;

public class TournamentSearchDto
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public bool? IsPublic { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}