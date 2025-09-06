using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.TournamentFormat;

public class TournamentFormatDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MinTeams { get; set; }
    public int MaxTeams { get; set; }
    public bool IsActive { get; set; }
}
