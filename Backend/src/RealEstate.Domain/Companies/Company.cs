using RealEstate.Domain.Common;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Companies;

public class Company : AuditableEntity
{
    public Company(Guid id) : base(id) { }
    public Guid? PocUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public User? PocUser { get; set; }
    public ICollection<User> Employees { get; set; } = new List<User>();

}
