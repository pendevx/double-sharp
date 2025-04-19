using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Music.Scripts.DS20.Common.MSSQL.Models;

public class Song : Entity
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
