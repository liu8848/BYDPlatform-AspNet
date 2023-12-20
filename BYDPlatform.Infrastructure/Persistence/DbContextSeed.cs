using BYDPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace BydPlatform.Infrastructure.Persistence;

public class DbContextSeed
{
    public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var administratorRole = new IdentityRole("Administrator");
        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }
        var administrator = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };
        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            // 创建的用户名为admin@localhost，密码是admin123，角色是Administrator
            await userManager.CreateAsync(administrator, "admin123");
            await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        }
    }
}