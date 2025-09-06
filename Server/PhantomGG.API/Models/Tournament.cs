using System;
using System.Collections.Generic;

namespace PhantomGG.API.Models;

public partial class Tournament
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Location { get; set; }

    public Guid FormatId { get; set; }

    public DateTime? RegistrationStartDate { get; set; }

    public DateTime? RegistrationDeadline { get; set; }

    public DateTime StartDate { get; set; }

    public int MinTeams { get; set; }

    public int MaxTeams { get; set; }

    public int MaxPlayersPerTeam { get; set; }

    public int MinPlayersPerTeam { get; set; }

    public decimal? EntryFee { get; set; }

    public decimal? PrizePool { get; set; }

    public string? ContactEmail { get; set; }

    public string? BannerUrl { get; set; }

    public string? LogoUrl { get; set; }

    public string Status { get; set; } = null!;

    public int? MatchDuration { get; set; }

    public Guid OrganizerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public bool IsPublic { get; set; }

    public virtual TournamentFormat Format { get; set; } = null!;

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual User Organizer { get; set; } = null!;

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
