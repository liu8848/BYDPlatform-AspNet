using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BYDPlatform.Domain.Base;
using BYDPlatform.Domain.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Domain.Entities;


[Table("business_division")]
public class BusinessDivision:AuditableEntity,IEntity<int>,IHasDomainEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    [Comment("主键-事业部编号")]
    public int Id { get; set; }
    
    [Column("bu_name")]
    [Comment("事业部名称")]
    public string BuName { get; set; }

    [NotMapped] public IList<RegisterFactory> RegisterFactories { get; private set; } = new List<RegisterFactory>();
    [NotMapped] public List<DomainEvent> DomainEvents { get; set; } = new();
}