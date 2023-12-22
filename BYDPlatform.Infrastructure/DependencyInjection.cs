using System.Text;
using Autofac;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Infrastructure.Extensions;
using BYDPlatform.Infrastructure.Identity;
using BydPlatform.Infrastructure.Persistence;
using BYDPlatform.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

        services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequiredLength = 6;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<BydPlatformDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration.GetSection("JwtSettings")["validIssuer"],
                    ValidAudience = configuration.GetSection("JwtSettings")["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration.GetSection("JwtSettings")["SecretKey"]?? "BYDPlatformApiSecretKey"))
                };
            });
        services.AddAuthorization(options =>
            options.AddPolicy("OnlyAdmin", policy => policy.RequireRole("Administrator")));

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        
        services.BatchRegisterService();
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