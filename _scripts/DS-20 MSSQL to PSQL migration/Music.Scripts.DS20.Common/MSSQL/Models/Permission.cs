using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Scripts.DS20.Common.MSSQL.Models;

public class Permission : Entity
{
    public Permission() : base()
    {
    }

    [StringLength(100)]
    [Unicode(false)]
    public string PermissionName { get; set; } = null!;

    [InverseProperty("Permission")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
