using System;
using System.Collections.Generic;

namespace PhantomGG.API.Models;

public partial class Team
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string ManagerName { get; set; } = null!;

    public string ManagerEmail { get; set; } = null!;

    public string? ManagerPhone { get; set; }

    public string? LogoUrl { get; set; }

    public string? TeamPhotoUrl { get; set; }

    public Guid TournamentId { get; set; }

    public string RegistrationStatus { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public int NumberOfPlayers { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Match> MatchAwayTeams { get; set; } = new List<Match>();

    public virtual ICollection<MatchEvent> MatchEvents { get; set; } = new List<MatchEvent>();

    public virtual ICollection<Match> MatchHomeTeams { get; set; } = new List<Match>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual Tournament Tournament { get; set; } = null!;
}
