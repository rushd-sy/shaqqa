using RealEstate.Domain.Bookings;
using RealEstate.Domain.Common;
using RealEstate.Domain.Properties;

namespace RealEstate.Domain.Bookings;

public class PropertyAvailability : AuditablePublicEntity
{
    public int PropertyId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public bool IsBooked { get; set; }

    public Property Property { get; set; } = null!;
    public ICollection<BookingVisit> Bookings { get; set; } = new List<BookingVisit>();
}