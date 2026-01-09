using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Scripts.DS20.Common.Postgres.Models;

[Index(nameof(Token), IsUnique = true)]
public class Session : BaseEntity
{
    public int AccountId { get; set; }

    public DateTime ExpiresOn { get; set; }

    public Guid Token { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Sessions")]
    public virtual Account Account { get; set; } = null!;
}
