using RealEstate.Domain.Amenities;
using RealEstate.Domain.Common;
using RealEstate.Domain.Properties;

namespace RealEstate.Domain.PropertyAmenities;

public class PropertyAmenity : InternalEntity
{
    public int PropertyId { get; set; }
    public int AmenityId { get; set; }

    public Property Property { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}