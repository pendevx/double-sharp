using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.Models.Data;

namespace Music.EntityFramework.ModelConfigurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> permission)
    {
        permission.HasKey(p => p.Id);

        permission.HasIndex(p => p.PermissionName)
            .IsUnique();

        permission.Property(p => p.PermissionName)
            .HasMaxLength(100);
    }
}
