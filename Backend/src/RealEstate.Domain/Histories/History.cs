using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Users;


namespace RealEstate.Domain.Histories;

public class History : AuditableEntity
{
    public History(Guid id) : base(id) { }
    public Guid UserId { get; set; }
    public Guid AdvertisementId { get; set; }
    public DateTime ViewedAt { get; set; }

    public User User { get; set; } = null!;
    public Advertisement Advertisement { get; set; } = null!;
}
