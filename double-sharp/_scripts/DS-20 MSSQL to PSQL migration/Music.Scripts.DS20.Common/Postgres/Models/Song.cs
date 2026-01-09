using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Music.Scripts.DS20.Common.Postgres.Models
{
    public class Song : BaseEntity
    {
        [StringLength(256)]
        public string Name { get; set; } = null!;

        [Column(TypeName = "oid")]
        public uint ContentsOid { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string MimeType { get; set; } = null!;

        public uint Length { get; set; }

        public Guid Guid { get; set; }
    }
}
