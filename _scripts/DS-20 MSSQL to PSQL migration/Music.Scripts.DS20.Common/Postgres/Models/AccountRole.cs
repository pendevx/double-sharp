﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Music.Scripts.DS20.Common.Postgres.Models;

public class AccountRole : BaseEntity
{
    public int AccountId { get; set; }

    public int RoleId { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("AccountRoles")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("AccountRoles")]
    public virtual Role Role { get; set; } = null!;
}
