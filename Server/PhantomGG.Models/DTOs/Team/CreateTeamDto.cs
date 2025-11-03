using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Team;

public class CreateTeamDto
{
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public IFormFile? LogoUrl { get; set; }
}
