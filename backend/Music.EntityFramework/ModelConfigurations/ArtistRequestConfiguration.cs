using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.Models.Data;

namespace Music.EntityFramework.ModelConfigurations;

public class ArtistRequestConfiguration : IEntityTypeConfiguration<ArtistRequest>
{
    public void Configure(EntityTypeBuilder<ArtistRequest> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(256);

        builder.Property(b => b.Status)
            .HasConversion<string>();

        builder.Property(b => b.Name)
            .HasMaxLength(256)
            .IsUnicode(false);

        builder.HasOne(b => b.Requester)
            .WithMany(requester => requester.ArtistRequests);

        builder.HasOne(b => b.Artist)
            .WithOne(artist => artist.ArtistRequest)
            .HasForeignKey((ArtistRequest b) => b.ArtistId)
            .HasPrincipalKey((Artist artist) => artist.Id);

        builder.Property(b => b.ArtistId)
            .IsRequired(false);
    }
}
