using Music.Models.Data.SongRequests;

namespace Music.Models.Data;

public class Account : BaseEntity
{
    public Guid Guid { get; set; }
    public string Username { get; set; } = null!;
    public byte[] SaltedPassword { get; set; } = null!;
    public string DisplayName { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
    public virtual ICollection<SongRequest> SongRequests { get; set; } = new List<SongRequest>();

    public bool HasAllRoles(params RoleName[] roles) =>
        roles.All(role => Roles.Select(r => r.Name).Contains(role.ToString()));
}
