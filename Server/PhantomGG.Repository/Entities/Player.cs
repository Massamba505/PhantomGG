using System;
using System.Collections.Generic;

namespace PhantomGG.Repository.Entities;

public partial class Player
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    public int Position { get; set; }

    public string? Email { get; set; }

    public Guid TeamId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<MatchEvent> MatchEvents { get; set; } = new List<MatchEvent>();

    public virtual Team Team { get; set; } = null!;
}
