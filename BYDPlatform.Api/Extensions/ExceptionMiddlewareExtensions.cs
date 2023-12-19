using BYDPlatform.Api.Middleware;

namespace BYDPlatform.Api.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }
}