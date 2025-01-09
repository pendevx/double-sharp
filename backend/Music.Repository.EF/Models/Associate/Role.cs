using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Repository.EF.Models.Generated;

[Index("Name", Name = "UQ_Name", IsUnique = true)]
public partial class Role : Entity
{
    [NotMapped]
    public RoleName NameEnum
    {
        get => Enum.Parse<RoleName>(Name);
        set => Name = value.ToString();
    }
}

public enum RoleName
{
    User,
    Admin,
}
