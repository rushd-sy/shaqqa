using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Locations;
using RealEstate.Domain.Properties.Enums;
using RealEstate.Domain.PropertyAmenities;
namespace RealEstate.Domain.Properties;

public class Property : AuditableInternalEntity
{
    public PropertyType PropertyType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? NumberOfRooms { get; set; }
    public int? FloorNumber { get; set; }
    public int LocationId { get; set; }
    public LegalStatus LegalStatus { get; set; }
    public DateTimeOffset ConstructionDate { get; set; }

    public Location Location { get; set; } = null!;
    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
}