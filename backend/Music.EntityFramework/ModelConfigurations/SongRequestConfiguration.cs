using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.Models.Data.SongRequests;

namespace Music.EntityFramework.ModelConfigurations;

public class SongRequestConfiguration : IEntityTypeConfiguration<SongRequest>
{
    public void Configure(EntityTypeBuilder<SongRequest> songRequest)
    {
        songRequest.HasKey(sr => sr.Id);
        songRequest.HasOne(sr => sr.Uploader).WithMany(sr => sr.SongRequests)
            .IsRequired();

        songRequest.Property(sr => sr.Name)
            .HasMaxLength(256)
            .IsUnicode(false);

        songRequest.Property(sr => sr.Status)
            .HasConversion<string>();

        songRequest.Property(info => info.Source)
            .HasConversion<string>()
            .HasColumnName("Source");

        songRequest.Property<string>("_urlValue")
            .HasColumnName("Url");

        songRequest.Ignore(sr => sr.Url);
        songRequest.Ignore(sr => sr.RawUrl);
    }
}
