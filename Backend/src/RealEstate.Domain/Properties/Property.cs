using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Locations;
using RealEstate.Domain.Properties.Enums;
using RealEstate.Domain.PropertyAmenities;
namespace RealEstate.Domain.Properties;

public class Property : AuditableEntity
{
    public Property(Guid id) : base(id) { }
    public Guid LocationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? UnitNumber { get; set; }
    public float Area { get; set; }
    public int? NumberOfRooms { get; set; }
    public int? FloorNumber { get; set; }
    public PropertyType PropertyType { get; set; }
    public LandType? LandType { get; set; }
    public ListingType ListingType { get; set; }
    public LegalStatus LegalStatus { get; set; }
    public PropertyStatus Status { get; set; }


    public Location Location { get; set; } = null!;
    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
}
