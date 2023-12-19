namespace BYDPlatform.Domain.Base.Interfaces;

public interface IHasDomainEvent
{
    public List<DomainEvent> DomainEvents { get; set; }
}