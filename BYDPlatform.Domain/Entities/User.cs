using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BYDPlatform.Domain.Base;
using BYDPlatform.Domain.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Domain.Entities;

[Table("user")]
[Comment("用户表")]
public class User : AuditableEntity, IEntity<int>, IHasDomainEvent
{
    [Column("user_name", TypeName = "varchar(20)")]
    [Required]
    [Comment("用户名")]
    public string UserName { get; set; }

    [Column("password", TypeName = "varchar(100)")]
    [Comment("密码")]
    [Required]
    public string Password { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id", TypeName = "int(11)")]
    [Comment("id主键")]
    public int Id { get; set; }

    [NotMapped] public List<DomainEvent> DomainEvents { get; set; } = new();
}