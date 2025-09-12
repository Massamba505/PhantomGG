using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Tournament;

public class UpdateTournamentDto
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

    [Range(4, 128)]
    public int MaxTeams { get; set; }

    [Range(0, 999999.99)]
    public decimal? EntryFee { get; set; }

    [Range(0, 999999.99)]
    public decimal? PrizePool { get; set; }

    [EmailAddress]
    public string? ContactEmail { get; set; }

    public string? BannerUrl { get; set; }

    public string? LogoUrl { get; set; }

    public string Status { get; set; } = string.Empty;

    public int? MatchDuration { get; set; }

    public bool IsPublic { get; set; } = true;
}
