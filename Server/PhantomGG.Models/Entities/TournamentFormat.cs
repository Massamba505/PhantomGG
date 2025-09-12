using System;
using System.Collections.Generic;

namespace PhantomGG.Models.Entities;

public partial class TournamentFormat
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int MinTeams { get; set; }

    public int MaxTeams { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
