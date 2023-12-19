using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BYDPlatform.Domain.Base;
using BYDPlatform.Domain.Base.Interfaces;
using BYDPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Domain.Entities;
[Table("register_factory")]
[Comment("备案工厂表")]
public class RegisterFactory:AuditableEntity,IEntity<int>,IHasDomainEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id",TypeName = "int(11)")]
    [Comment("主键")]
    public int Id { get; set; }

    [Column("bu_id",TypeName = "int(11)")]
    [Comment("所属事业部编号")]
    public int BuId { get; set; }
    
    [Column("factory_name",TypeName = "varchar(50)")]
    [Comment("备案工厂名称")]
    public string FactoryName { get; set; }
    
    [Column("factory_level",TypeName = "int(2)")]
    [Comment("工厂等级")]
    public FactoryLevel Level { get; set; }
    
    [NotMapped] public List<DomainEvent> DomainEvents { get; set; } = new();
}