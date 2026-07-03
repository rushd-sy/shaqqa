// Property.cs
using RealEstate.Domain.Common;
using RealEstate.Domain.RealEstate.Enume;
using RealEstate.Domain.User;
namespace RealEstate.Domain.RealEstate;

public class RealEstate : AuditableEntity
{
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public RealEstateType PropertyType { get; set; }
    public LandType? LandType { get; set; }
    public ListingType ListingType { get; set; }
    public LegalStatus LegalStatus { get; set; }
    public RealEstateStatus Status { get; set; }

    public decimal Price { get; set; }
    public double Area { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<VerificationRequest> VerificationRequests { get; set; } = new List<VerificationRequest>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}