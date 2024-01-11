using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;

namespace BYDPlatform.Infrastructure.Log.Enrichers;

public class HttpContextEnricher:ILogEventEnricher
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Action<LogEvent, ILogEventPropertyFactory, HttpContext> _enricherAction;
    
    public HttpContextEnricher(IServiceProvider serviceProvider):this(serviceProvider,null){}

    public HttpContextEnricher(IServiceProvider serviceProvider,
        Action<LogEvent, ILogEventPropertyFactory, HttpContext> enricherAction)
    {
        _serviceProvider = serviceProvider;
        if (enricherAction == null)
        {
            _enricherAction = (logEvent, propertyFactory, httpContext) =>
            {
                var x_forwarded_for = new StringValues();
                httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out x_forwarded_for);

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("client_ip",
                    JsonConvert.SerializeObject(x_forwarded_for)));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("request_path", httpContext.Request.Path));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("request_method",
                    httpContext.Request.Method));
                if (httpContext.Response.HasStarted)
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("response_status",
                        httpContext.Response.StatusCode));
                }
            };
        }
        else
        {
            _enricherAction = enricherAction;
        }
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        if (null != httpContext){
            _enricherAction.Invoke(logEvent, propertyFactory, httpContext);
        }
    }
}