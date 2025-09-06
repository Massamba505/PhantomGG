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

    [Required]
    public Guid FormatId { get; set; }

    public DateTime? RegistrationStartDate { get; set; }

    public DateTime? RegistrationDeadline { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Range(2, 64)]
    public int MinTeams { get; set; } = 2;

    [Range(4, 128)]
    public int MaxTeams { get; set; } = 16;

    [Range(7, 25)]
    public int MaxPlayersPerTeam { get; set; } = 11;

    [Range(7, 25)]
    public int MinPlayersPerTeam { get; set; } = 7;

    [Range(0, 999999.99)]
    public decimal? EntryFee { get; set; }

    [Range(0, 999999.99)]
    public decimal? PrizePool { get; set; }

    [EmailAddress]
    public string? ContactEmail { get; set; }

    public string? BannerUrl { get; set; }

    public string? LogoUrl { get; set; }

    [Range(60, 120)]
    public int MatchDuration { get; set; } = 90;

    public bool IsPublic { get; set; } = true;
}
