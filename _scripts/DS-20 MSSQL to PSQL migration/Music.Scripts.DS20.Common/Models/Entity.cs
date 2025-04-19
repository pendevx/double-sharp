using System.ComponentModel.DataAnnotations;

namespace Music.Scripts.DS20.Common.Models;

public class Entity
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }
}
