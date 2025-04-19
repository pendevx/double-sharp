using System.ComponentModel.DataAnnotations;

namespace Music.Scripts.DS20.Common.MSSQL.Models;

public class Entity
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }
}
