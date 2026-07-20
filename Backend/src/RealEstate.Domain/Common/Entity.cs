namespace RealEstate.Domain.Common;
public abstract class Entity
{
    public Guid Id { get;  } 
    
    private readonly List<DomainEvent> _domainEvents = [];

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid(): id;
    }
    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
   public void ClearDomainEvent()
    {
        _domainEvents.Clear();
    }
  
}