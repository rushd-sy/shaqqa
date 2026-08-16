using RealEstate.Domain.Common;
using RealEstate.Domain.PropertyAmenities;

namespace RealEstate.Domain.Amenities;

public class Amenity : AuditableInternalEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
}