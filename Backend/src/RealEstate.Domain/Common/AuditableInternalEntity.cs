namespace RealEstate.Domain.Common;

public abstract class AuditableInternalEntity : InternalEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}