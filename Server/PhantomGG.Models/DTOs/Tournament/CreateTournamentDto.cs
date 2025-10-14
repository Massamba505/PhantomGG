using Microsoft.AspNetCore.Http;

namespace PhantomGG.Models.DTOs.Tournament;

public class CreateTournamentDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime RegistrationStartDate { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MinTeams { get; set; } = 2;
    public int MaxTeams { get; set; } = 16;
    public IFormFile? BannerUrl { get; set; }
    public IFormFile? LogoUrl { get; set; }
    public bool IsPublic { get; set; } = true;
}
