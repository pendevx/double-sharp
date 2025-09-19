using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.Models.Data;

namespace Music.EntityFramework.ModelConfigurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> song)
    {
        song.HasKey(s => s.Id);

        song.Property(s => s.Name)
            .HasMaxLength(256);

        song.Ignore(s => s.Artists);

        song.HasMany<Artist>("_artists")
            .WithMany("_songs");
    }
}
