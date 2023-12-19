using BYDPlatform.Application.Common.Interfaces;
using BYDPlatform.Domain.Base;
using Microsoft.Extensions.Logging;

namespace BYDPlatform.Infrastructure.Services;

public class DomainEventService : IDomainEventService
{
    public required ILogger<DomainEventService> _logger { protected get; init; }

    public DomainEventService(ILogger<DomainEventService> logger)
    {
        Serilog.Log.Information("注入服务");
        _logger = logger;
    }

    public async Task Publish(DomainEvent domainEvent)
    {
        _logger.LogInformation("Publishing domain event,Event - {event}", domainEvent.GetType().Name);
    }
}