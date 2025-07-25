namespace PhantomGG.API.Models;

public partial class Player
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }

    public string FirstName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string ProfilePictureUrl { get; set; } = null!;

    public bool IsCaptain { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PlayerStat> PlayerStats { get; set; } = new List<PlayerStat>();

    public virtual Team Team { get; set; } = null!;
}
