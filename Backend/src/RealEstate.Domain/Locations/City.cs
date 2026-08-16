using RealEstate.Domain.Common;
using RealEstate.Domain.Locations;

namespace RealEstate.Domain.Locations;

public class City : AuditablePublicEntity
{
    public City() { }

    public City(int id, Guid publicId)
    {
        Id = id;
        PublicId = publicId;
    }

    public int CountryId { get; set; }
    public string Name { get; set; } = string.Empty;

    public Country Country { get; set; } = null!;
    public ICollection<Location> Locations { get; set; } = new List<Location>();
}