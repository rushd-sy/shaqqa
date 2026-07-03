// PropertyImage.cs
using RealEstate.Domain.Common;

namespace RealEstate.Domain.RealEstate.RealEstateImage;

public class PropertyImage : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public RealEstate Property { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}
