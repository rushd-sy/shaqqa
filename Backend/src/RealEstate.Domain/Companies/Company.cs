using RealEstate.Domain.Common;
using RealEstate.Domain.Locations;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Companies;

public class Company : AuditablePublicEntity
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int? IdLocation { get; set; }
    public bool IsActive { get; set; } = true;

    public Location? Location { get; set; }
    public ICollection<User> Employees { get; set; } = new List<User>();
}