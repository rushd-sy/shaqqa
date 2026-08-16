namespace RealEstate.Domain.Common;

public abstract class PublicEntity : InternalEntity
{
    public Guid PublicId { get; protected set; }

    protected PublicEntity()
    {
        PublicId = Guid.CreateVersion7().ToSqlServerSequential();
    }

    protected PublicEntity(Guid publicId)
    {
        PublicId = publicId == Guid.Empty ? Guid.CreateVersion7().ToSqlServerSequential() : publicId;
    }
}