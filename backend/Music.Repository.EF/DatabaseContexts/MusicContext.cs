using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Music.Repository.EF.Models.Generated;

namespace Music.Repository.EF.DatabaseContexts;

public partial class MusicContext : DbContext
{
    public MusicContext(DbContextOptions<MusicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountRole> AccountRoles { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<SongRequest> SongRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accounts__3214EC070724D861");

            entity.Property(e => e.DisplayName).HasDefaultValue("");
        });

        modelBuilder.Entity<AccountRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AccountR__3214EC072CCAAC36");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccountRo__Accou__6477ECF3");

            entity.HasOne(d => d.Role).WithMany(p => p.AccountRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccountRo__RoleI__656C112C");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC075C7B230F");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC075DCC548E");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolePerm__3214EC07C2F3826A");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions).HasConstraintName("FK__RolePermi__Permi__68487DD7");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions).HasConstraintName("FK__RolePermi__RoleI__693CA210");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sessions__3214EC073CC5A28E");

            entity.HasOne(d => d.Account).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Accoun__4CA06362");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Songs__3214EC076C077814");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<SongRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SongRequ__3214EC0767956C50");

            entity.HasOne(d => d.UploaderAccount).WithMany(p => p.SongRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SongReque__Uploa__5CD6CB2B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
