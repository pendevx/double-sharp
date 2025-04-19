using System.ComponentModel.DataAnnotations.Schema;

namespace Music.Scripts.DS20.Common.Models;

public class Session : Entity
{
    public Session() : base()
    {
    }

    public int AccountId { get; set; }

    public DateTime ExpiresOn { get; set; }

    public Guid Token { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Sessions")]
    public virtual Account Account { get; set; } = null!;
}
