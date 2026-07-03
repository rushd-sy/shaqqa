// Enums/NotificationType.cs
namespace RealEstate.Domain.Enums;

public enum NotificationType
{
    VerificationApproved = 1,
    VerificationRejected = 2,
    VerificationNeedsEdits = 3,
    NewReport = 4,
    PropertySold = 5,
    PropertyRented = 6
}