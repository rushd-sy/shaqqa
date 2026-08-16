using RealEstate.Domain.Common;
using RealEstate.Domain.Advertisements;

namespace RealEstate.Domain.StoredFiles;

public class StoredFile : AuditablePublicEntity
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StoredPath { get; set; } = string.Empty;

    public ICollection<Media> Media { get; set; } = new List<Media>();
}