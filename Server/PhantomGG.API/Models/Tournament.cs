namespace PhantomGG.API.Models;

public partial class Tournament
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Format { get; set; } = null!;

    public Guid OrganizerId { get; set; }

    public bool IsPublic { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly RegistrationDeadline { get; set; }

    public string Status { get; set; } = null!;

    public string BannerImageUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual User Organizer { get; set; } = null!;

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
