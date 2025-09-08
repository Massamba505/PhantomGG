using PhantomGG.API.Common;

namespace PhantomGG.API.DTOs.Player;

public class PlayerSearchDto
{
    public string? SearchTerm { get; set; }
    public Guid? TeamId { get; set; }
    public Guid? TournamentId { get; set; }
    public PlayerPosition? Position { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
