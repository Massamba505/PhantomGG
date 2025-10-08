using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Models.DTOs.Team;

public class TournamentTeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? LogoUrl { get; set; }
    public TeamRegistrationStatus Status { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public string? ManagerName { get; set; }
    public Guid? ManagerId { get; set; }
    public IEnumerable<PlayerDto> Players { get; set; } = new List<PlayerDto>();

}