namespace PhantomGG.Models.DTOs.Team;

public class TournamentTeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? LogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public string? ManagerName { get; set; }
    public Guid? ManagerId { get; set; }
}