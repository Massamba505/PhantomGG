namespace PhantomGG.Models.DTOs.Team;

public class TeamSearchDto
{
    public string? SearchTerm { get; set; }
    public Guid? TournamentId { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
