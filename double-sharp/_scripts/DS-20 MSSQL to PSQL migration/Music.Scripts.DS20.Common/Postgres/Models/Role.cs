using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Scripts.DS20.Common.Postgres.Models;

[Index(nameof(Name), IsUnique = true)]
public class Role : BaseEntity
{
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();

    [InverseProperty("Role")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
