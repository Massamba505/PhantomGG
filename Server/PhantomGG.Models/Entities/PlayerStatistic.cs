using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.Entities;

public partial class PlayerStatistic
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public Guid TournamentId { get; set; }

    // Appearance Statistics
    public int MatchesPlayed { get; set; } = 0;

    public int MatchesStarted { get; set; } = 0;

    public int SubstitutionsIn { get; set; } = 0;

    public int SubstitutionsOut { get; set; } = 0;

    public int MinutesPlayed { get; set; } = 0;

    // Goal Statistics
    public int Goals { get; set; } = 0;

    public int Assists { get; set; } = 0;

    public int PenaltyGoals { get; set; } = 0;

    public int FreeKickGoals { get; set; } = 0;

    public int HeaderGoals { get; set; } = 0;

    public int OwnGoals { get; set; } = 0;

    // Disciplinary
    public int YellowCards { get; set; } = 0;

    public int RedCards { get; set; } = 0;

    // Goalkeeper Statistics
    public int CleanSheets { get; set; } = 0;

    public int GoalsConceded { get; set; } = 0;

    public int Saves { get; set; } = 0;

    public int PenaltiesSaved { get; set; } = 0;

    // Performance Rating
    public decimal? AverageRating { get; set; }

    // Metadata
    public DateTime LastUpdated { get; set; }

    // Navigation properties
    public virtual Player Player { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
