using Microsoft.EntityFrameworkCore;
using Music.EntityFramework.ModelConfigurations;
using Music.Models.Data;
using Music.Models.Data.SongRequests;

namespace Music.EntityFramework;

public sealed class MusicContext : DbContext
{
    public MusicContext(DbContextOptions<MusicContext> options) : base(options) { }

    public MusicContext() { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<SongRequest> SongRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());

        modelBuilder.Entity<Song>(songBuilder =>
        {
            songBuilder.HasKey(s => s.Id);

            songBuilder.Property(s => s.Name)
                .HasMaxLength(256);
        });

        modelBuilder.Entity<SongRequest>(songRequestBuilder =>
        {
            songRequestBuilder.HasKey(sr => sr.Id);
            songRequestBuilder.HasOne(sr => sr.Uploader).WithMany(sr => sr.SongRequests)
                .IsRequired();

            songRequestBuilder.Property(sr => sr.Name)
                .HasMaxLength(256)
                .IsUnicode(false);

            songRequestBuilder.Property(sr => sr.Status)
                .HasConversion<string>();

            songRequestBuilder.Property(info => info.Source)
                .HasConversion<string>()
                .HasColumnName("Source");

            songRequestBuilder.Property<string>("_urlValue")
                .HasColumnName("Url");

            songRequestBuilder.Ignore(sr => sr.Url);
            songRequestBuilder.Ignore(sr => sr.RawUrl);
        });

        modelBuilder.Entity<Role>(role =>
        {
            role.HasKey(r => r.Id);

            role.Property(r => r.Name)
                .HasMaxLength(50);

            role.HasIndex(r => r.Name)
                .IsUnique();
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DOUBLESHARP_DB_CONNECTION_STRING"));
        base.OnConfiguring(optionsBuilder);
    }
}
