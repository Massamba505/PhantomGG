using System;
using System.Collections.Generic;

namespace PhantomGG.API.Models;

public partial class Match
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public string? Venue { get; set; }

    public Guid HomeTeamId { get; set; }

    public Guid AwayTeamId { get; set; }

    public DateTime MatchDate { get; set; }

    public string Status { get; set; } = null!;

    public int? HomeScore { get; set; }

    public int? AwayScore { get; set; }

    public virtual Team AwayTeam { get; set; } = null!;

    public virtual Team HomeTeam { get; set; } = null!;

    public virtual ICollection<MatchEvent> MatchEvents { get; set; } = new List<MatchEvent>();

    public virtual Tournament Tournament { get; set; } = null!;
}
