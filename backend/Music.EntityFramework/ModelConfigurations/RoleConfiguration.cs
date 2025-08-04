using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.Models.Data;

namespace Music.EntityFramework.ModelConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> role)
    {
        role.HasKey(r => r.Id);

        role.Property(r => r.Name)
            .HasConversion<string>();

        role.HasIndex(r => r.Name)
            .IsUnique();

        role.HasMany(r => r.Permissions)
            .WithMany(p => p.Roles);

        role.HasData(RolesTable);
    }

    public static IEnumerable<Role> RolesTable =>
    [
        new() { Id = 1, Name = RoleName.User },
        new() { Id = 2, Name = RoleName.Admin },
    ];
}
