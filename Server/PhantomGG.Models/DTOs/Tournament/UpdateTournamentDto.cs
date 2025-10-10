using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Tournament;

public class UpdateTournamentDto
{
    [StringLength(200, MinimumLength = 3)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    public DateTime? RegistrationDeadline { get; set; }

    public DateTime? StartDate { get; set; }

    [Range(4, 128)]
    public int? MaxTeams { get; set; }

    [EmailAddress]
    public string? ContactEmail { get; set; }

    public IFormFile? BannerUrl { get; set; }

    public IFormFile? LogoUrl { get; set; }

    public bool? IsPublic { get; set; }
}