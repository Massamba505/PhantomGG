using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Team;

public class UpdateTeamDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(10)]
    public string? ShortName { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string ManagerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string ManagerEmail { get; set; } = string.Empty;

    [StringLength(10)]
    public string? ManagerPhone { get; set; }

    public string? LogoUrl { get; set; }

    public string? TeamPhotoUrl { get; set; }
}
