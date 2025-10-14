using Microsoft.AspNetCore.Http;

namespace PhantomGG.Models.DTOs.Tournament;

public class UpdateTournamentDto
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    public DateTime? RegistrationStartDate { get; set; }

    public DateTime? RegistrationDeadline { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MinTeams { get; set; }

    public int? MaxTeams { get; set; }

    public IFormFile? BannerUrl { get; set; }

    public IFormFile? LogoUrl { get; set; }

    public bool? IsPublic { get; set; }
}