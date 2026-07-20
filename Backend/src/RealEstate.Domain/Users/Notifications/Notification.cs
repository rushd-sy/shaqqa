using RealEstate.Domain.Common;
using RealEstate.Domain.Enquiries;
using RealEstate.Domain.Users.Notifications.Enums;

namespace RealEstate.Domain.Users.Notifications;

public class Notification : AuditableEntity
{
    public Notification(Guid id) : base(id) { }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? EnquiryId { get; set; }
    public Enquiry? Enquiry { get; set; }

    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }

    public DateTime? SentAt { get; set; }
}
