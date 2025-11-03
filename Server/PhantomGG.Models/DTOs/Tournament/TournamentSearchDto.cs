using PhantomGG.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Tournament;

public class TournamentQuery
{
    public string? Q { get; set; }
    public TournamentStatus? Status { get; set; }
    public string? Location { get; set; }
    public DateTime? StartFrom { get; set; }
    public DateTime? StartTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IsPublic { get; set; } = true;
}