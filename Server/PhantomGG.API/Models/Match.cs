namespace PhantomGG.API.Models;

public partial class Match
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Guid HomeTeamId { get; set; }

    public Guid AwayTeamId { get; set; }

    public DateTime ScheduledTime { get; set; }

    public string Venue { get; set; } = null!;

    public string Status { get; set; } = null!;

    public short? HomeTeamScore { get; set; }

    public short? AwayTeamScore { get; set; }

    public virtual Team AwayTeam { get; set; } = null!;

    public virtual Team HomeTeam { get; set; } = null!;

    public virtual ICollection<PlayerStat> PlayerStats { get; set; } = new List<PlayerStat>();

    public virtual Tournament Tournament { get; set; } = null!;
}
