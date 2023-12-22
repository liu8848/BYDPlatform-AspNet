using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BYDPlatform.Api.Filters;

public class LogFilterAttribute:IActionFilter
{
    private readonly ILogger<LogFilterAttribute> _logger;

    public LogFilterAttribute(ILogger<LogFilterAttribute> logger) => _logger = logger;

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
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];

        if (context.Exception is not null)
        {
            context.Result = new BadRequestObjectResult(context.Exception);
        }
        var result = (ObjectResult)context.Result!;
        _logger.LogInformation($"Controller:{controller}, action: {action}, Executing response: {JsonSerializer.Serialize(result.Value)}");
    }
    
}