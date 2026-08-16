using RealEstate.Domain.Common;
using RealEstate.Domain.StoredFiles;

namespace RealEstate.Domain.Advertisements;

public class Media : AuditablePublicEntity
{
    public int AdvertisementId { get; set; }
    public int FileId { get; set; }
    public bool IsCover { get; set; }
    public int DisplayOrder { get; set; }

    public Advertisement Advertisement { get; set; } = null!;
    public StoredFile File { get; set; } = null!;
}