using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Repository.EF.Models.Generated;

[Index("PermissionName", Name = "UQ__Permissi__0FFDA357DAD6BA83", IsUnique = true)]
public partial class Permission : Entity
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
