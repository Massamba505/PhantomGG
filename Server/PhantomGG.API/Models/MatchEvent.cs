using System;
using System.Collections.Generic;

namespace PhantomGG.API.Models;

public partial class MatchEvent
{
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }

    public string EventType { get; set; } = null!;

    public int Minute { get; set; }

    public Guid TeamId { get; set; }

    public string? PlayerName { get; set; }

    public string? Description { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
