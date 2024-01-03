using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BYDPlatform.Domain.Attributes;
using BYDPlatform.Domain.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BYDPlatform.Domain.Entities;

[Entity]
[Table("operation_log")]
public class OperationLog : IEntity<long>
{
    [Column("ip_address", TypeName = "varchar(50)")]
    [Comment("发送请求ip")]
    public string? IpAddress { get; set; }

    [Column("user", TypeName = "varchar(50)")]
    [Comment("发送请求用户名")]
    public string User { get; set; }

    [Column("executed_time", TypeName = "datetime")]
    [Comment("执行时间")]
    public DateTime ExecutedTime { get; set; }

    [Column("controller", TypeName = "varchar(100)")]
    [Comment("控制器名称")]
    public string? Controller { get; set; }

    [Column("action", TypeName = "varchar(100)")]
    [Comment("执行方法")]
    public string? Action { get; set; }

    [Column("execute_status")]
    [Comment("方法执行状态（0:失败,1:成功")]
    public bool ExecuteStatus { get; set; }

    [Column("exception_type", TypeName = "varchar(100)")]
    [Comment("抛出错误类型")]
    public string? ExceptionType { get; set; }

    [Column("exception_msg", TypeName = "varchar(200)")]
    [Comment("抛出错误信息")]
    public string? ExceptionMsg { get; set; }

    [Column("request_params", TypeName = "varchar(100)")]
    [Comment("请求参数")]
    public string? RequestParams { get; set; }

    [Column("response_body", TypeName = "varchar(1000)")]
    [Comment("响应体信息")]
    public string? ResponseBody { get; set; }

    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Comment("操作日志主键Id")]
    public long Id { get; set; }
}