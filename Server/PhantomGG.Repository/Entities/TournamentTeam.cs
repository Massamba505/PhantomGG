using System;
using System.Collections.Generic;

namespace PhantomGG.Repository.Entities;

public partial class TournamentTeam
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Guid TeamId { get; set; }

    public int Status { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
