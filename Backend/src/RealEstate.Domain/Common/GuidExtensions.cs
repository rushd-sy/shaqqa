using System.Buffers.Binary;

namespace RealEstate.Domain.Common;

public static class GuidExtensions
{
    public static Guid ToSqlServerSequential(this Guid guid)
    {
        Span<byte> bytes = stackalloc byte[16];
        guid.TryWriteBytes(bytes);

        Span<byte> sqlBytes = stackalloc byte[16];

        bytes[6..16].CopyTo(sqlBytes[0..10]);
        bytes[0..6].CopyTo(sqlBytes[10..16]);

        return new Guid(sqlBytes);
    }
}