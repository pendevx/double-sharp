using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Music.Models.Data.SongRequests;

namespace Music.Models.Data;

[Index(nameof(Username), IsUnique = true)]
public class Account : BaseEntity
{
    public Guid Guid { get; set; }

    [StringLength(256)]
    public string Username { get; set; } = null!;

    public byte[] SaltedPassword { get; set; } = null!;

    [StringLength(30)]
    public string DisplayName { get; set; } = null!;

    [InverseProperty("Account")]
    public ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();

    [InverseProperty("Account")]
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    public ICollection<SongRequest> SongRequests { get; set; } = new List<SongRequest>();
}
