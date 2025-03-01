using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Models.Data
{
    [Index(nameof(PermissionName), IsUnique = true)]
    public class Permission : BaseEntity
    {
        [StringLength(100)]
        public string PermissionName { get; set; } = null!;

        [InverseProperty("Permission")]
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
