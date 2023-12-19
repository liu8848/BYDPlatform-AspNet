using Autofac;
using BYDPlatform.Application.Common.Interfaces;
using BydPlatform.Infrastructure.Persistence;
using BYDPlatform.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BYDPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BydPlatformDbContext>(options =>
        {
            Serilog.Log.Information("================= 开始建立数据库连接 =============================");
            options.UseMySql(
                configuration.GetConnectionString("MySqlConnection"),
                new MySqlServerVersion(new Version(8, 0, 34)),
                b => b.MigrationsAssembly(typeof(BydPlatformDbContext).Assembly.FullName)
            );
        });

        services.AddScoped<IDomainEventService, DomainEventService>();
        return services;
    }

    public static void AddInfrastructure(this ContainerBuilder builder,
        IConfiguration configuration)
    {
        builder.Register(options =>
        {
            var optionBuilder = new DbContextOptionsBuilder<BydPlatformDbContext>();
            optionBuilder.UseMySql(
                configuration.GetConnectionString("MySqlConnection")!,
                new MySqlServerVersion(new Version(8, 0, 34)),
                b => b.MigrationsAssembly(typeof(BydPlatformDbContext).Assembly.FullName)
            );
            return optionBuilder.Options;
        }).InstancePerLifetimeScope();
        
        builder.RegisterType<DomainEventService>().As<IDomainEventService>().InstancePerLifetimeScope();

        builder.RegisterType<BydPlatformDbContext>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}