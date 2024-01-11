using BYDPlatform.Infrastructure.Log.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BYDPlatform.Infrastructure.Log;

public static class ConfigureLogProvider
{
    public static readonly string FileMessageTemplate = 
        "{NewLine}Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}IP:{request_path}{NewLine}ThreadId:{ThreadId}{NewLine}LogLevel：{Level}{NewLine}SourceContext:{SourceContext} {NewLine}Message：{Message:l}{NewLine}{Exception}" + new string('-', 100);
    public static void ConfigureLog(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue<bool>("UseFileToLog"))
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: FileMessageTemplate)
                .WriteTo.File(
                    "logs/log-.txt",
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 15)
                .CreateLogger();
        else
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: FileMessageTemplate)
                .CreateLogger();

        builder.Host.UseSerilog();
    }
    
    public static IApplicationBuilder UseHttpContextLog(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HttpContextLogMiddleware>();
    }
}