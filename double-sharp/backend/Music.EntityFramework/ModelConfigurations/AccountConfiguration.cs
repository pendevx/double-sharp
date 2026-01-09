using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.Models.Data;

namespace Music.EntityFramework.ModelConfigurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> account)
    {
        account.HasKey(a => a.Id);

        account.HasIndex(a => a.Username)
            .IsUnique();

        account.Property(a => a.Username)
            .HasMaxLength(256);

        account.Property(a => a.DisplayName)
            .HasMaxLength(30);

        account.HasMany(a => a.Sessions)
            .WithOne(s => s.Account);

        account.HasMany(a => a.SongRequests)
            .WithOne(sr => sr.Uploader);

        account.HasMany(a => a.Roles)
            .WithMany(r => r.Accounts);
    }
}
