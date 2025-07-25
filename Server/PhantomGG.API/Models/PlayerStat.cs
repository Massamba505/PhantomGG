namespace PhantomGG.API.Models;

public partial class PlayerStat
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public Guid MatchId { get; set; }

    public short Goals { get; set; }

    public short Assists { get; set; }

    public short YellowCards { get; set; }

    public short RedCards { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
