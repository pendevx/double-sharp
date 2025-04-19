using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Scripts.DS20.Common.Postgres.Models;

[Index(nameof(PermissionName), IsUnique = true)]
public class Permission : BaseEntity
{
    [StringLength(100)]
    public string PermissionName { get; set; } = null!;

    [InverseProperty("Permission")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
