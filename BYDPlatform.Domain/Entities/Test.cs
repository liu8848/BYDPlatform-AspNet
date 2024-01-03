using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BYDPlatform.Domain.Attributes;

namespace BYDPlatform.Domain.Entities;

[Entity]
[Table("test")]
public class Test
{
    [Column("id", TypeName = "int(11)")]
    [Key]
    public int Id { get; set; }
}