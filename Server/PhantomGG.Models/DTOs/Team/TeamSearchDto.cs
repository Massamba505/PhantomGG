using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.Team;

public class TeamQuery
{
    public string? Q { get; set; }
    public Guid? TournamentId { get; set; }
    public TeamRegistrationStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public bool? IsPublic { get; set; }
}
