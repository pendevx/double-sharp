using System.ComponentModel.DataAnnotations;

namespace Music.Models.Data;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
