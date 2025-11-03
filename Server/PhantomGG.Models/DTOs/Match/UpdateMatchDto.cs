using PhantomGG.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Match;

public class UpdateMatchDto
{
    public DateTime MatchDate { get; set; }
    public MatchStatus Status { get; set; }
}
