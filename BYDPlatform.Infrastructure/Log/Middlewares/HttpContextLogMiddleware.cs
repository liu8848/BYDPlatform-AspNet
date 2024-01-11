using BYDPlatform.Infrastructure.Log.Enrichers;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace BYDPlatform.Infrastructure.Log.Middlewares;

public  class HttpContextLogMiddleware
{
    private readonly RequestDelegate _next;

    public HttpContextLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var serviceProvider = context.RequestServices;
        using (LogContext.Push(new HttpContextEnricher(serviceProvider)))
        {
            await _next(context);
        }
    }
}