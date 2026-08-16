using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Favorites;

public class Favorite : AuditablePublicEntity
{
    public Guid UserId { get; set; }
    public int AdvertisementId { get; set; }

    public User User { get; set; } = null!;
    public Advertisement Advertisement { get; set; } = null!;
}