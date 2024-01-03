using System.Reflection;
using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Domain.Attributes;
using BYDPlatform.Domain.Base;
using BYDPlatform.Domain.Base.Interfaces;
using BYDPlatform.Domain.Entities;
using BYDPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BydPlatform.Infrastructure.Persistence;

public class BydPlatformDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventService _domainEventService;

    public BydPlatformDbContext(DbContextOptions<BydPlatformDbContext> options,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService) : base(options)
    {
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RegisterFactory> RegisterFactories => Set<RegisterFactory>();
    public DbSet<BusinessDivision> BusinessDivisions => Set<BusinessDivision>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = string.IsNullOrEmpty(_currentUserService.UserName)
                        ? "Anonymous"
                        : _currentUserService.UserName;
                    entry.Entity.Created = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = string.IsNullOrEmpty(_currentUserService.UserName)
                        ? "Anonymous"
                        : _currentUserService.UserName;
                    entry.Entity.LastModified = DateTime.UtcNow;
                    break;
            }

        var events = ChangeTracker.Entries<IHasDomainEvent>()
            .Select(x => x.Entity.DomainEvents)
            .SelectMany(x => x)
            .Where(domainEvent => !domainEvent.IsPublished)
            .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);
        return result;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var types = GetAllAssemblyEntities();
        foreach (var type in types) modelBuilder.Entity(type);
    }

    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }

    private List<Type> GetAllAssemblyEntities()
    {
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        HashSet<string> loadedAssemblies = new();

        foreach (var item in allAssemblies) loadedAssemblies.Add(item.FullName!);

        Queue<Assembly> assembliesToCheck = new();
        assembliesToCheck.Enqueue(Assembly.GetEntryAssembly()!);

        while (assembliesToCheck.Any())
        {
            var assemblyToCheck = assembliesToCheck.Dequeue();
            foreach (var reference in assemblyToCheck!.GetReferencedAssemblies())
                if (!loadedAssemblies.Contains(reference.FullName))
                {
                    var assembly = Assembly.Load(reference);
                    assembliesToCheck.Enqueue(assembly);
                    loadedAssemblies.Add(reference.FullName);
                    allAssemblies.Add(assembly);
                }
        }

        var types = allAssemblies.SelectMany(t => t.GetTypes())
            .Where(
                t =>
                    t.GetCustomAttributes(typeof(EntityAttribute), false).Length > 0
                    && t.IsClass && !t.IsAbstract
            ).ToList();
        return types;
    }
}