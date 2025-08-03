namespace Music.Models.Data;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}

public enum RoleName
{
    User,
    Admin,
}
