using RealEstate.Domain.Common;
using RealEstate.Domain.Users.Notifications.Enums;

namespace RealEstate.Domain.Users.Notifications;

public class Notification : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}