using RealEstate.Domain.Common;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Bookings;

public class BookingVisit : AuditablePublicEntity
{
    public Guid UserId { get; set; }
    public int PropertyId { get; set; }
    public DateTimeOffset AppointmentDatetime { get; set; }
    public BookingStatus Status { get; set; }
    public int AvailabilityId { get; set; }

    public User User { get; set; } = null!;
    public Property Property { get; set; } = null!;
    public PropertyAvailability Availability { get; set; } = null!;
}