using RealEstate.Domain.Common;
using RealEstate.Domain.Locations;

namespace RealEstate.Domain.Locations;

public class Country : AuditablePublicEntity
{
    public Country() { }

    public Country(int id, Guid publicId)
    {
        Id = id;
        PublicId = publicId;
    }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public ICollection<City> Cities { get; set; } = new List<City>();
    public ICollection<Location> Locations { get; set; } = new List<Location>();
}