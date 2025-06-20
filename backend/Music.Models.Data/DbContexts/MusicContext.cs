using Microsoft.EntityFrameworkCore;

namespace Music.Models.Data.DbContexts;

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
        modelBuilder.Entity<SongRequest>(b =>
        {
            b.Property(sr => sr.SourceUrl)
                .HasMaxLength(2048)
                .IsUnicode(false);

            b.Property(sr => sr.Source)
                .HasMaxLength(50)
                .IsUnicode(false);

            b.Property(sr => sr.Name)
                .HasMaxLength(256)
                .IsUnicode(false);

            b.Property(sr => sr.MimeType)
                .HasMaxLength(50)
                .IsUnicode(false);

            b.Property(sr => sr.RequestStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<RequestStatus>(v));
        });
    }
}
