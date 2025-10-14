using Microsoft.AspNetCore.Http;

namespace PhantomGG.Models.DTOs.Team;

public class UpdateTeamDto
{
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public IFormFile? LogoUrl { get; set; }
    public IFormFile? TeamPhotoUrl { get; set; }
}
