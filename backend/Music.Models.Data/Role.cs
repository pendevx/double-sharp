namespace Music.Models.Data;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}

public enum RoleName
{
    User,
    Admin,
}
