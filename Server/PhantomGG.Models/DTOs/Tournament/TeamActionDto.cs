using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.Tournament;

public class TeamActionDto
{
    public TeamAction Action { get; set; }
    public Guid? TeamId { get; set; }
    public string? Reason { get; set; }
}