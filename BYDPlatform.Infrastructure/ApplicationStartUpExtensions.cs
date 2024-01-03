using BYDPlatform.Infrastructure.Identity;
using BydPlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BYDPlatform.Infrastructure;

public static class ApplicationStartUpExtensions
{
    public static async Task MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<BydPlatformDbContext>();
            context.Database.Migrate();

            // //认证
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await DbContextSeed.SeedDefaultUserAsync(userManager, roleManager);
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred migration the DB:{e.Message}");
        }
    }
}