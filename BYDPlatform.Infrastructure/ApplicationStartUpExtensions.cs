using Autofac;
using BydPlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BYDPlatform.Infrastructure;

public static class ApplicationStartUpExtensions
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<BydPlatformDbContext>();
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred migration the DB:{e.Message}");
        }
    }
    
}