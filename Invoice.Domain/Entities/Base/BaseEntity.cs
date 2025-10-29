
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoice.Domain.Entities.Base;

public abstract class BaseEntity : IEntity
{
    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped] public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public int Id { get; set; }

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

