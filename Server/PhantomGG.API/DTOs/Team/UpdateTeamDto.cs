using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Team;

public class UpdateTeamDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Manager { get; set; } = string.Empty;

    [Range(1, 50)]
    public int NumberOfPlayers { get; set; }

    public string? LogoUrl { get; set; }
}
