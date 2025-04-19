using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Scripts.DS20.Common.MSSQL.Models;

public class SongRequest : Entity
{
    [StringLength(100)]
    [Unicode(false)]
    public string RequestStatus { get; set; } = null!;

    [StringLength(2048)]
    [Unicode(false)]
    public string SourceUrl { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Source { get; set; } = null!;

    public int UploaderAccountId { get; set; }

    [StringLength(256)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MimeType { get; set; }

    [ForeignKey("UploaderAccountId")]
    [InverseProperty("SongRequests")]
    public virtual Account UploaderAccount { get; set; } = null!;
}
