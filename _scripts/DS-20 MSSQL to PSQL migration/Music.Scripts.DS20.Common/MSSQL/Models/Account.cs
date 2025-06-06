using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music.Scripts.DS20.Common.MSSQL.Models;

public class Account : Entity
{
    public Account() : base()
    {
    }

    public Guid Guid { get; set; }

    [StringLength(256)]
    public string Username { get; set; } = null!;

    public byte[] SaltedPassword { get; set; } = null!;

    [StringLength(30)]
    public string DisplayName { get; set; } = null!;

    [InverseProperty("Account")]
    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();

    [InverseProperty("Account")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    [InverseProperty("UploaderAccount")]
    public virtual ICollection<SongRequest> SongRequests { get; set; } = new List<SongRequest>();
}
