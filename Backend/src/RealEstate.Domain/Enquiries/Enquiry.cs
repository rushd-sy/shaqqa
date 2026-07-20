using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Users;
using RealEstate.Domain.Users.Notifications;

namespace RealEstate.Domain.Enquiries;

public class Enquiry : AuditableEntity
{
    public Enquiry(Guid id) : base(id) { }
    public Guid EnquiryId { get; set; }
    public Guid UserId { get; set; }
    public Guid AdvertisementId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Advertisement Advertisement { get; set; } = null!;
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
