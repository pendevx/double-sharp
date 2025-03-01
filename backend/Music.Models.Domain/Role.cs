namespace Music.Models.Domain;

public class Role : Models.Data.Role
{
    public RoleName NameEnum
    {
        get => Enum.Parse<RoleName>(Name);
        set => Name = value.ToString();
    }
}

public enum RoleName
{
    User,
    Admin,
}
