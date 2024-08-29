﻿using System.ComponentModel.DataAnnotations;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Index("Guid", Name = "UQ__Songs__A2B5777D47B8A67B", IsUnique = true)]
public partial class Song : Entity
{
    [Key]
    public int Id { get; set; }

    public Guid Guid { get; set; }

    [StringLength(256)]
    public string Name { get; set; } = null!;

    public byte[] Contents { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MimeType { get; set; } = null!;
}
