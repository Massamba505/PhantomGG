namespace PhantomGG.API.DTOs.Tournament;

public class TournamentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxTeams { get; set; }
    public decimal? EntryFee { get; set; }
    public decimal? Prize { get; set; }
    public string? ContactEmail { get; set; }
    public string? BannerUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid Organizer { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int TeamCount { get; set; }
}
