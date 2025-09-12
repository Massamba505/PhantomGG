using Microsoft.EntityFrameworkCore;
using PhantomGG.Models.Entities;

namespace PhantomGG.Repository.Data;

public partial class PhantomContext : DbContext
{
    public PhantomContext()
    {
    }

    public PhantomContext(DbContextOptions<PhantomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<MatchEvent> MatchEvents { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<TournamentFormat> TournamentFormats { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasIndex(e => e.MatchDate, "IX_Matches_MatchDate");

            entity.HasIndex(e => e.TournamentId, "IX_Matches_TournamentId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Scheduled");
            entity.Property(e => e.Venue)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.AwayTeam).WithMany(p => p.MatchAwayTeams)
                .HasForeignKey(d => d.AwayTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Matches_AwayTeam");

            entity.HasOne(d => d.HomeTeam).WithMany(p => p.MatchHomeTeams)
                .HasForeignKey(d => d.HomeTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Matches_HomeTeam");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Matches)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Matches_Tournament");
        });

        modelBuilder.Entity<MatchEvent>(entity =>
        {
            entity.HasIndex(e => e.MatchId, "IX_MatchEvents_MatchId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EventType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PlayerName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Match).WithMany(p => p.MatchEvents)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK_MatchEvents_Match");

            entity.HasOne(d => d.Team).WithMany(p => p.MatchEvents)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchEvents_Team");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasIndex(e => e.IsActive, "IX_Players_IsActive");

            entity.HasIndex(e => new { e.FirstName, e.LastName }, "IX_Players_Name");

            entity.HasIndex(e => e.Position, "IX_Players_Position");

            entity.HasIndex(e => e.TeamId, "IX_Players_TeamId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhotoUrl).IsUnicode(false);
            entity.Property(e => e.Position)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Team).WithMany(p => p.Players)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_Players_Team");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_RefreshTokens_UserId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasIndex(e => e.IsActive, "IX_Teams_IsActive");

            entity.HasIndex(e => e.ManagerName, "IX_Teams_ManagerName");

            entity.HasIndex(e => e.Name, "IX_Teams_Name");

            entity.HasIndex(e => e.RegistrationStatus, "IX_Teams_RegistrationStatus");

            entity.HasIndex(e => e.ShortName, "IX_Teams_ShortName");

            entity.HasIndex(e => e.TournamentId, "IX_Teams_TournamentId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LogoUrl).IsUnicode(false);
            entity.Property(e => e.ManagerEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ManagerName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ManagerPhone)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RegistrationStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.ShortName)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TeamPhotoUrl).IsUnicode(false);

            entity.HasOne(d => d.Tournament).WithMany(p => p.Teams)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Teams_Tournament");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasIndex(e => e.FormatId, "IX_Tournaments_FormatId");

            entity.HasIndex(e => e.IsActive, "IX_Tournaments_IsActive");

            entity.HasIndex(e => e.IsPublic, "IX_Tournaments_IsPublic");

            entity.HasIndex(e => e.OrganizerId, "IX_Tournaments_Organizer");

            entity.HasIndex(e => e.StartDate, "IX_Tournaments_StartDate");

            entity.HasIndex(e => e.Status, "IX_Tournaments_Status");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BannerUrl).IsUnicode(false);
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.EntryFee)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPublic).HasDefaultValue(true);
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.LogoUrl).IsUnicode(false);
            entity.Property(e => e.MatchDuration).HasDefaultValue(90);
            entity.Property(e => e.MaxPlayersPerTeam).HasDefaultValue(11);
            entity.Property(e => e.MaxTeams).HasDefaultValue(16);
            entity.Property(e => e.MinPlayersPerTeam).HasDefaultValue(7);
            entity.Property(e => e.MinTeams).HasDefaultValue(2);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PrizePool)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Draft");

            entity.HasOne(d => d.Format).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.FormatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tournaments_Format");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tournaments_Organizer");
        });

        modelBuilder.Entity<TournamentFormat>(entity =>
        {
            entity.HasIndex(e => e.IsActive, "IX_TournamentFormats_IsActive");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxTeams).HasDefaultValue(32);
            entity.Property(e => e.MinTeams).HasDefaultValue(2);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Organizer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
