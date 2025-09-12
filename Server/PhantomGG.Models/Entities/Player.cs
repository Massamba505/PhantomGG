using System;
using System.Collections.Generic;

namespace PhantomGG.Models.Entities;

public partial class Player
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Position { get; set; }

    public string? Email { get; set; }

    public string? PhotoUrl { get; set; }

    public Guid TeamId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Team Team { get; set; } = null!;
}
