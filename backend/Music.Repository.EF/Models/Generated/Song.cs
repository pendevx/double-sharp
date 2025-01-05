using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Repository.EF.Models.Generated;

[Index("Guid", Name = "UQ__Songs__A2B5777D47B8A67B", IsUnique = true)]
public partial class Song : Entity
{
    public Song() : base()
    {
    }

    public Guid Guid { get; set; }

    [StringLength(256)]
    public string Name { get; set; } = null!;

    public byte[] Contents { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MimeType { get; set; } = null!;
}
