using BYDPlatform.Api.Extensions;
using BYDPlatform.Api.Filters;
using BYDPlatform.Application;
using BYDPlatform.Application.Common.Implements;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Infrastructure;
using BYDPlatform.Infrastructure.Log;
using BYDPlatform.Infrastructure.Repositories;
using Serilog;

namespace BYDPlatform.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureLog();
        Log.Information("===================== 启动项目:BYD_Platform ======================");

        // builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
        //     .ConfigureContainer<ContainerBuilder>(b =>
        //     {
        //         b.RegisterType<DomainEventService>().As<IDomainEventService>().InstancePerLifetimeScope();
        //     });

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddScoped<LogFilterAttribute>();


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();

        builder.Services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));

        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();


        var app = builder.Build();

        app.UseHttpContextLog();
        
        app.UseGlobalExceptionHandler();


        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MigrateDatabase();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}