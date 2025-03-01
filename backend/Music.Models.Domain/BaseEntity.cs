using System.ComponentModel.DataAnnotations;

namespace Music.Models.Domain;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
