using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.Models.Data;

namespace Music.EntityFramework.ModelConfigurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> session)
    {
        session.HasKey(s => s.Id);

        session.HasIndex(s => s.Token)
            .IsUnique();

        session.HasOne(s => s.Account)
            .WithMany(a => a.Sessions);
    }
}
