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

        artist.Property(a => a._slug).HasMaxLength(50);
        artist.HasIndex(a => a._slug).IsUnique();
        artist.Property(a => a._slug).HasColumnName("Slug");

        artist.Ignore(a => a.Slug);

        artist.HasMany(a => a.Songs).WithMany(s => s.Artists);

        artist.OwnsOne<RequestInformation>("_requestInformation", requestInfo =>
        {
            requestInfo.Property(ri => ri.Status)
                .HasConversion<string>()
                .HasColumnName("RequestStatus");

            requestInfo.HasOne(r => r.Requester)
                .WithMany()
                .HasForeignKey("_requesterId");

            requestInfo.Property<int>("_requesterId")
                .HasColumnName("RequesterId");
        });
        artist.Navigation("_requestInformation").IsRequired(false);
        artist.Ignore(a => a.RequestInformation);
    }
}
