using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.Models.Data;

namespace Music.EntityFramework.ModelConfigurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> artist)
    {
        artist.HasKey(b => b.Id);

        artist.Property(b => b.Name)
            .HasMaxLength(256);

        artist.HasMany(a => a.Songs)
            .WithMany(s => s.Artists);

        artist.OwnsOne<RequestInformation>("_requestInformation", requestInfo =>
        {
            requestInfo.Property(ri => ri.Status)
                .HasConversion<string>();
        });
        artist.Navigation("_requestInformation").IsRequired(false);
        artist.Ignore(a => a.RequestInformation);
    }
}
