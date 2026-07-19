using RealEstate.Domain.Common;

namespace RealEstate.Domain.Properties.PropertyImages;

public class PropertyImage : AuditableEntity
{
    public PropertyImage(Guid id) : base(id)
    {
    }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}
