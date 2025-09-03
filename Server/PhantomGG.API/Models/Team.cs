using System;
using System.Collections.Generic;

namespace PhantomGG.API.Models;

public partial class Team
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Manager { get; set; } = null!;

    public int NumberOfPlayers { get; set; }

    public string? LogoUrl { get; set; }

    public Guid TournamentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Tournament Tournament { get; set; } = null!;
}
