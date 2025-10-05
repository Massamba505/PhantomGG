using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Entities;

namespace PhantomGG.Repository.Data;

public partial class PhantomContext : DbContext
{
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

    public virtual DbSet<TournamentTeam> TournamentTeams { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasIndex(e => e.MatchDate, "IX_Matches_MatchDate");

            entity.HasIndex(e => e.TournamentId, "IX_Matches_TournamentId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AwayScore).HasDefaultValue(0);
            entity.Property(e => e.HomeScore).HasDefaultValue(0);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Scheduled");

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
            entity.Property(e => e.EventType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Match).WithMany(p => p.MatchEvents)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK_MatchEvents_Match");

            entity.HasOne(d => d.Player).WithMany(p => p.MatchEvents)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchEvents_Player");

            entity.HasOne(d => d.Team).WithMany(p => p.MatchEvents)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchEvents_Team");
        });

        modelBuilder.Entity<Player>(entity =>
        {
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
            entity.HasIndex(e => e.Name, "IX_Teams_Name");

            entity.HasIndex(e => e.ShortName, "IX_Teams_ShortName");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LogoUrl).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ShortName)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Teams)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teams_User");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasIndex(e => e.IsPublic, "IX_Tournaments_IsPublic");

            entity.HasIndex(e => e.Name, "IX_Tournaments_Name");

            entity.HasIndex(e => e.OrganizerId, "IX_Tournaments_Organizer");

            entity.HasIndex(e => e.StartDate, "IX_Tournaments_StartDate");

            entity.HasIndex(e => e.Status, "IX_Tournaments_Status");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BannerUrl).IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsPublic).HasDefaultValue(true);
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.LogoUrl).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Draft");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tournaments_Organizer");
        });

        modelBuilder.Entity<TournamentTeam>(entity =>
        {
            entity.HasIndex(e => e.Status, "IX_TournamentTeams_Status");

            entity.HasIndex(e => e.TeamId, "IX_TournamentTeams_Team");

            entity.HasIndex(e => e.TournamentId, "IX_TournamentTeams_Tournament");

            entity.HasIndex(e => new { e.TournamentId, e.TeamId }, "UQ_TournamentTeams_Unique").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RequestedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Team).WithMany(p => p.TournamentTeams)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_TournamentTeams_Team");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentTeams)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_TournamentTeams_Tournament");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.AccountLockedUntil, "IX_Users_AccountLockedUntil");

            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.HasIndex(e => e.EmailVerificationToken, "IX_Users_EmailVerificationToken");

            entity.HasIndex(e => e.PasswordResetToken, "IX_Users_PasswordResetToken");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmailVerificationToken)
                .HasMaxLength(255)
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
            entity.Property(e => e.PasswordResetToken)
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
