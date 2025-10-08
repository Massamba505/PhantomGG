using PhantomGG.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Team;

public class TeamQuery
{
    public string? Q { get; set; }
    public Guid? TournamentId { get; set; }
    public TeamRegistrationStatus? Status { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    public int Page { get; set; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Page size must be at least 1")]
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public bool? IsPublic { get; set; }
}
