using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Models.Domain;

[Index("Token", Name = "UQ__Sessions__1EB4F817BC4B8EF7", IsUnique = true)]
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
