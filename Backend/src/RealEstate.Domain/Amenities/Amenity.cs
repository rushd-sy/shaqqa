using RealEstate.Domain.Common;
using RealEstate.Domain.PropertyAmenities;

namespace RealEstate.Domain.Amenities;

public class Amenity : AuditableEntity
{
    public Amenity(Guid id) : base(id) { }
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
}
