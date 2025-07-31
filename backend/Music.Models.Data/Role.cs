namespace Music.Models.Data;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public enum RoleName
{
    User,
    Admin,
}
