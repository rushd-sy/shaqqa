namespace RealEstate.Domain.Common;
public abstract class Entity
{
    public Guid Id { get;  } 
    
    private readonly List<DomainEvent> _domainEvent = [];
    protected Entity()
    {
        
    }
      protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid(): id;
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvent.Remove(domainEvent);
    }
   public void ClearDomainEvent()
    {
        _domainEvent.Clear();
    }
  
}