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
    public DbSet<Artist> Artists { get; set; }
    public DbSet<ArtistRequest> ArtistRequests { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<SongRequest> SongRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new ArtistConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new SongConfiguration());
        modelBuilder.ApplyConfiguration(new SongRequestConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DOUBLESHARP_DB_CONNECTION_STRING"));
        base.OnConfiguring(optionsBuilder);
    }
}
