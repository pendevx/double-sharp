using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Repository.EF.Models.Generated;

[Index("Token", Name = "UQ__Sessions__1EB4F817BC4B8EF7", IsUnique = true)]
public partial class Session : Entity
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
