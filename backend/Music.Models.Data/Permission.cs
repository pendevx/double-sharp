namespace Music.Models.Data;

public class Permission : BaseEntity
{
    protected Permission() { }

    public string PermissionName { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
