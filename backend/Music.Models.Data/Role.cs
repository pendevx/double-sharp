namespace Music.Models.Data;

public class Role : BaseEntity
{
    public RoleName Name { get; set; }

    public static implicit operator RoleName(Role role) => role.Name;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}

public enum RoleName
{
    User,
    Admin,
}
