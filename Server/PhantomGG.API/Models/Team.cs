namespace PhantomGG.API.Models;

public partial class Team
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public string Name { get; set; } = null!;

    public string LogoUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Match> MatchAwayTeams { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchHomeTeams { get; set; } = new List<Match>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual Tournament Tournament { get; set; } = null!;
}
