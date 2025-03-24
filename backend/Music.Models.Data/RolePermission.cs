using System.ComponentModel.DataAnnotations.Schema;

namespace Music.Models.Data;

public class RolePermission : BaseEntity
{
    public int? PermissionId { get; set; }

    public int? RoleId { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("RolePermissions")]
    public virtual Permission? Permission { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("RolePermissions")]
    public virtual Role? Role { get; set; }
}
