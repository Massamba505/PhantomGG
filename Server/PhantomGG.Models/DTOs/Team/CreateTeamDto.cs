using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Team;

public class CreateTeamDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(10)]
    public string? ShortName { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string ManagerName { get; set; } = string.Empty;

    public IFormFile? LogoUrl { get; set; }

    public IFormFile? TeamPhotoUrl { get; set; }

    [Required]
    public Guid TournamentId { get; set; }
}
