using System.Reflection;
using BYDPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BydPlatform.Infrastructure.Persistence;

public class UserDbContext:DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options):base(options)
    {
    }

    public DbSet<User> Users => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("user");
        
    }
}