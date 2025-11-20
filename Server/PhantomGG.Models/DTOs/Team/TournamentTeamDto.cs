using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Models.DTOs.Team;

public class TournamentTeamDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid TournamentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? LogoUrl { get; set; }
    public string? TournamentName { get; set; }
    public TeamRegistrationStatus Status { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public string? ManagerName { get; set; }
    public Guid? ManagerId { get; set; }
    public int CountPlayers { get; set; } = 0;
    public IEnumerable<PlayerDto> Players { get; set; } = new List<PlayerDto>();

}