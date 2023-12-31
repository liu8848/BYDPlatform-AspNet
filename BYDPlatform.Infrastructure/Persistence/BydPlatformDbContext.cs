using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Domain.Base;
using BYDPlatform.Domain.Base.Interfaces;
using BYDPlatform.Domain.Entities;
using BYDPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BydPlatform.Infrastructure.Persistence;

public class BydPlatformDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;

    public BydPlatformDbContext(DbContextOptions<BydPlatformDbContext> options,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService) : base(options)
    {
        Log.Information($"注入服务:{typeof(BydPlatformDbContext).FullName}");
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
                    entry.Entity.CreatedBy = _currentUserService.UserName;
                    entry.Entity.Created = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserName;
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
        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<RegisterFactory>().ToTable("register_factory");
        modelBuilder.Entity<BusinessDivision>();
        
    }

    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }
}