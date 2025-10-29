namespace Invoice.Domain.Entities.Base;

public abstract class BaseEvent
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}

