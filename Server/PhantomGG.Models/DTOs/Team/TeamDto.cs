namespace PhantomGG.Models.DTOs.Team;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public string ManagerEmail { get; set; } = string.Empty;
    public string? ManagerPhone { get; set; }
    public string? LogoUrl { get; set; }
    public string? TeamPhotoUrl { get; set; }
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public string RegistrationStatus { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public int NumberOfPlayers { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
