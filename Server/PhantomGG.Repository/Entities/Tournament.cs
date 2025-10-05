using System;
using System.Collections.Generic;

namespace PhantomGG.Repository.Entities;

public partial class Tournament
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? BannerUrl { get; set; }

    public string? LogoUrl { get; set; }

    public DateTime RegistrationStartDate { get; set; }

    public DateTime RegistrationDeadline { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int MinTeams { get; set; }

    public int MaxTeams { get; set; }

    public string Status { get; set; } = null!;

    public Guid OrganizerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsPublic { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual User Organizer { get; set; } = null!;

    public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();
}
