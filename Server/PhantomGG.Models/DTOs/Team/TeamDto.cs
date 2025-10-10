using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Models.DTOs.Team;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CountPlayers { get; set; } = 0;
    public IEnumerable<PlayerDto> players { get; set; } = new List<PlayerDto>();
}
