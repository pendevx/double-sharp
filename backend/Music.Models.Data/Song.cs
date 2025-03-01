using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Music.Models.Data
{
    public class Song : BaseEntity
    {
        public Guid Guid { get; set; } // to be removed, psql doesn't require this

        [StringLength(256)]
        public string Name { get; set; } = null!;

        public int ContentsOid { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string MimeType { get; set; } = null!;
    }
}
