using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Team;

public class CreateTeamDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Manager { get; set; } = string.Empty;

    [Range(1, 50)]
    public int NumberOfPlayers { get; set; } = 1;

    public string? LogoUrl { get; set; }

    [Required]
    public Guid TournamentId { get; set; }
}
