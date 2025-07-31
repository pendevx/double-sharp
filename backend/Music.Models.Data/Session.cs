namespace Music.Models.Data;

public class Session : BaseEntity
{
    public DateTime ExpiresOn { get; set; }
    public Guid Token { get; set; }
    public virtual Account Account { get; set; } = null!;
}
