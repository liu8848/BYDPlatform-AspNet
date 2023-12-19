using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Domain.Base;

public abstract class AuditableEntity
{
    [Column("created")] [Comment("创建时间")] public DateTime Created { get; set; }

    [Column("created_by")]
    [Comment("创建人")]
    public string? CreatedBy { get; set; }

    [Column("last_modified")]
    [Comment("修改时间")]
    public DateTime? LastModified { get; set; }

    [Column("last_modified_by")]
    [Comment("上次修改时间")]
    public string? LastModifiedBy { get; set; }
}