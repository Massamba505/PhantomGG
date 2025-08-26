using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Tournament;

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

    public DateTime? RegistrationDeadline { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(4, 128)]
    public int MaxTeams { get; set; } = 16;

    [Range(0, 999999.99)]
    public decimal? EntryFee { get; set; }

    [Range(0, 999999.99)]
    public decimal? Prize { get; set; }

    [EmailAddress]
    public string? ContactEmail { get; set; }

    public string? BannerUrl { get; set; }
}
