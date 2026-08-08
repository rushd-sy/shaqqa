using RealEstate.Domain.Common;
using RealEstate.Domain.Properties;

namespace RealEstate.Domain.Locations;

public class Location : AuditableEntity
{
    public Location(Guid id) : base(id) { }

    public string? PlaceId { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
