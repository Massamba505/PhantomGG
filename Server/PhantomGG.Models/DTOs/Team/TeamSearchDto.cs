using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.Team;

public class TeamSearchDto
{
    public string? SearchTerm { get; set; }
    public Guid? TournamentId { get; set; }
    public TeamRegistrationStatus? RegistrationStatus { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
