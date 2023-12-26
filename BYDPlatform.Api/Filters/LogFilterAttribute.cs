using System.Text.Json;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BYDPlatform.Api.Filters;

public class LogFilterAttribute:IActionFilter
{
    private readonly ILogger<LogFilterAttribute> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<OperationLog> _repository;

    public LogFilterAttribute(
        ILogger<LogFilterAttribute> logger,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        IRepository<OperationLog> repository
        )
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _repository = repository;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        //获取执行方法
        var action = context.RouteData.Values["action"];
        //获取控制器
        var controller = context.RouteData.Values["controller"];
        //获取传入参数
        var param = context.ActionArguments.SingleOrDefault(x=>x.Value.ToString().Contains("Command")).Value;
        
        _logger.LogInformation($"Controller:{controller}, action:{action},Incoming request:{JsonSerializer.Serialize(param)}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var operationLog = new OperationLog();

        //执行方法
        operationLog.Action = context.RouteData.Values["action"]?.ToString();
        //执行控制器
        operationLog.Controller= context.RouteData.Values["controller"]?.ToString();
        //获取请求ip
        operationLog.IpAddress= _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        //获取当前请求用户名
        operationLog.User = string .IsNullOrEmpty(_currentUserService.UserName)?"anyone":_currentUserService.UserName;
        //执行时间
        operationLog.ExecutedTime=DateTime.UtcNow;
        //
        operationLog.RequestParams = context.HttpContext.Request.Path;

        if (context.Exception is not null)
        {
            
            operationLog.ExecuteStatus = false;
            operationLog.ExceptionType = context.Exception.GetType().FullName;
            operationLog.ExceptionMsg = context.Exception.Message;
            _repository.AddAsync(operationLog);
            throw context.Exception;
        }

        operationLog.ExecuteStatus = true;
        var result = (ObjectResult)context.Result!;
        //响应体
        operationLog.ResponseBody =JsonSerializer.Serialize(result.Value).ToString() ;
        
        _repository.AddAsync(operationLog);
        
        _logger.LogInformation($"Controller:{operationLog.Controller}, " +
                               $"action: {operationLog.Action}, " +
                               $"ipAddress:{operationLog.IpAddress},"+
                               $"user:{operationLog.User},"+
                               $"Executing response: {JsonSerializer.Serialize(result.Value)}");
    }
    
}