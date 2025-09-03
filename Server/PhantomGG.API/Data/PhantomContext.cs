using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Models;

namespace PhantomGG.API.Data;

public partial class PhantomContext : DbContext
{
    public PhantomContext()
    {
    }

    public PhantomContext(DbContextOptions<PhantomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            entity.HasIndex(e => e.Name, "IX_Teams_Name");

            entity.HasIndex(e => e.TournamentId, "IX_Teams_TournamentId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LogoUrl).IsUnicode(false);
            entity.Property(e => e.Manager)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NumberOfPlayers).HasDefaultValue(1);

            entity.HasOne(d => d.Tournament).WithMany(p => p.Teams)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Teams_TournamentId");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasIndex(e => e.IsActive, "IX_Tournaments_IsActive");

            entity.HasIndex(e => e.Organizer, "IX_Tournaments_Organizer");

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
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.MaxTeams).HasDefaultValue(9);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Prize)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.OrganizerNavigation).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.Organizer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tournaments_Organizer");
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
