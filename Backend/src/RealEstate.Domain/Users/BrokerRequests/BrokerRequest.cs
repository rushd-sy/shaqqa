using RealEstate.Domain.Common;
using RealEstate.Domain.Locations;
using RealEstate.Domain.Users;
using RealEstate.Domain.Users.BrokerRequests;

namespace RealEstate.Domain.Users.BrokerRequests;

public class BrokerRequest : AuditablePublicEntity
{
    public Guid UserId { get; set; }
    public bool PriorExperience { get; set; }
    public int? IdLocation { get; set; }
    public BrokerRequestStatus Status { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? RequestNotes { get; set; }

    public User User { get; set; } = null!;
    public Location? Location { get; set; }
    public User? ReviewedByUser { get; set; }
}