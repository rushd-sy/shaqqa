using RealEstate.Domain.Amenities;
using RealEstate.Domain.Common;
using RealEstate.Domain.Properties;

namespace RealEstate.Domain.PropertyAmenities;

public class PropertyAmenity : AuditableEntity
{
    public PropertyAmenity(Guid id) : base(id) { }
    public Guid PropertyId { get; set; }
    public Guid AmenityId { get; set; }

    public Property Property { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}
