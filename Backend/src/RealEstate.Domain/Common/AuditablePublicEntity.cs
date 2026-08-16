namespace RealEstate.Domain.Common;

public abstract class AuditablePublicEntity : PublicEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}