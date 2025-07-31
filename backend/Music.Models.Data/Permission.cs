namespace Music.Models.Data;

public class Permission : BaseEntity
{
    public string PermissionName { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
