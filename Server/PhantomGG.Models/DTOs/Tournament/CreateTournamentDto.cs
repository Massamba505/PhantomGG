using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Tournament;

public class CreateTournamentDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Location { get; set; }

    public DateTime? RegistrationStartDate { get; set; }

    public DateTime? RegistrationDeadline { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(2, 64)]
    public int MinTeams { get; set; } = 2;

    [Range(4, 128)]
    public int MaxTeams { get; set; } = 16;

    public IFormFile? BannerUrl { get; set; }

    public IFormFile? LogoUrl { get; set; }

    public bool IsPublic { get; set; } = true;
}
