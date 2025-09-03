using System;
using System.Collections.Generic;

namespace PhantomGG.API.Models;

public partial class Tournament
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Location { get; set; }

    public DateTime? RegistrationDeadline { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int MaxTeams { get; set; }

    public decimal? EntryFee { get; set; }

    public decimal? Prize { get; set; }

    public string? ContactEmail { get; set; }

    public string? BannerUrl { get; set; }

    public string Status { get; set; } = null!;

    public Guid Organizer { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual User OrganizerNavigation { get; set; } = null!;

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
