using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Models;

namespace PhantomGG.API.Data;

public partial class PhantomGGContext : DbContext
{
    public PhantomGGContext()
    {
    }

    public PhantomGGContext(DbContextOptions<PhantomGGContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerStat> PlayerStats { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Matches__3214EC0731ABE440");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AwayTeamScore).HasDefaultValue((short)0);
            entity.Property(e => e.HomeTeamScore).HasDefaultValue((short)0);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("scheduled");
            entity.Property(e => e.Venue).HasMaxLength(100);

            entity.HasOne(d => d.AwayTeam).WithMany(p => p.MatchAwayTeams)
                .HasForeignKey(d => d.AwayTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_AwayTeam");

            entity.HasOne(d => d.HomeTeam).WithMany(p => p.MatchHomeTeams)
                .HasForeignKey(d => d.HomeTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_HomeTeam");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Matches)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Match_Tournament");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Players__3214EC075828BB0F");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(255);
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Team).WithMany(p => p.Players)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_Player_Team");
        });

        modelBuilder.Entity<PlayerStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlayerSt__3214EC079BA31BFF");

            entity.HasIndex(e => new { e.PlayerId, e.MatchId }, "PlayerPerMatch").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Match).WithMany(p => p.PlayerStats)
                .HasForeignKey(d => d.MatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerStats)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("FK_Player");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07ACB1DBAC");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.TokenHash).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Teams__3214EC07A47A36E4");

            entity.HasIndex(e => new { e.TournamentId, e.Name }, "TeamPerTournament").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LogoUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(80);

            entity.HasOne(d => d.Tournament).WithMany(p => p.Teams)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Team_Tournament");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC07691BDA51");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BannerImageUrl).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Format).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPublic).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("upcoming");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.OrganizerId)
                .HasConstraintName("FK_Tournament_Organizer");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0728861A53");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105346CE29E30").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordSalt).HasMaxLength(128);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Organizer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
