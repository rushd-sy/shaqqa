using RealEstate.Domain.Common;
using RealEstate.Domain.Companies;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Users.BrokerRequests;

namespace RealEstate.Domain.Locations;

public class Location : AuditablePublicEntity
{
    public string? PlaceId { get; set; }
    public int? CityId { get; set; }
    public int? CountryId { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public City? City { get; set; }
    public Country? Country { get; set; }
    public Property? Property { get; set; }
    public ICollection<Company> Companies { get; set; } = new List<Company>();
    public ICollection<BrokerRequest> BrokerRequests { get; set; } = new List<BrokerRequest>();
}