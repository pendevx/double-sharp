using Microsoft.EntityFrameworkCore;
using Music.Scripts.DS20.Common.Models;

namespace Music.Scripts.DS20.Common;

public class MusicContext : DbContext
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
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DisplayName);
        });

        modelBuilder.Entity<AccountRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Account).WithMany(p => p.AccountRoles)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Role).WithMany(p => p.AccountRoles)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions);

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Account).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedOn);
        });

        modelBuilder.Entity<SongRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.UploaderAccount).WithMany(p => p.SongRequests)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
    }
}
