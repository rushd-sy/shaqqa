using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.AdvertisementViews;

public class AdvertisementView : AuditableInternalEntity
{
    public Guid UserId { get; set; }
    public int AdvertisementId { get; set; }
    public DateTimeOffset ViewedAt { get; set; }

    public User User { get; set; } = null!;
    public Advertisement Advertisement { get; set; } = null!;
}