namespace PhantomGG.Models.DTOs.Tournament;

public class TournamentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime? RegistrationStartDate { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MinTeams { get; set; }
    public int MaxTeams { get; set; }
    public string? BannerUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsPublic { get; set; }
    public int TeamCount { get; set; }
    public int MatchCount { get; set; }
}
