namespace PhantomGG.Models.DTOs.Tournament;

public class TournamentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public Guid FormatId { get; set; }
    public string FormatName { get; set; } = string.Empty;
    public DateTime? RegistrationStartDate { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public DateTime StartDate { get; set; }
    public int MinTeams { get; set; }
    public int MaxTeams { get; set; }
    public int MaxPlayersPerTeam { get; set; }
    public int MinPlayersPerTeam { get; set; }
    public decimal? EntryFee { get; set; }
    public decimal? PrizePool { get; set; }
    public string? ContactEmail { get; set; }
    public string? BannerUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MatchDuration { get; set; }
    public Guid OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }
    public int TeamCount { get; set; }
    public int MatchCount { get; set; }
    public int CompletedMatches { get; set; }
}
