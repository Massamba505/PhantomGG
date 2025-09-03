namespace PhantomGG.API.DTOs.Team;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Manager { get; set; } = string.Empty;
    public int NumberOfPlayers { get; set; }
    public string? LogoUrl { get; set; }
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
