using Music.Models.Data.SongRequests;

namespace Music.Models.Data;

public class Account : BaseEntity
{
    public Guid Guid { get; set; }
    public string Username { get; set; } = null!;
    public byte[] SaltedPassword { get; set; } = null!;
    public string DisplayName { get; set; } = null!;

    public ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<SongRequest> SongRequests { get; set; } = new List<SongRequest>();
}
