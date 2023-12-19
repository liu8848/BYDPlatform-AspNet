using BYDPlatform.Domain.Base;

namespace BYDPlatform.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}