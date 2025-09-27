using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Team;

public class RejectTeamDto
{
    [StringLength(500)]
    public string? Reason { get; set; }
}