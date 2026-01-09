using System.ComponentModel.DataAnnotations;

namespace Music.Scripts.DS20.Common.Postgres.Models;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
