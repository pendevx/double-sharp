using System.ComponentModel.DataAnnotations.Schema;

namespace Music.Scripts.DS20.Common.MSSQL.Models;

public class RolePermission : Entity
{
    public RolePermission() : base()
    {
    }

    public int? PermissionId { get; set; }

    public int? RoleId { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("RolePermissions")]
    public virtual Permission? Permission { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("RolePermissions")]
    public virtual Role? Role { get; set; }
}
