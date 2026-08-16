using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using RealEstate.Domain.Common;

namespace RealEstate.Infrastructure.Persistence.ValueGenerators;

public class SqlServerUuidV7Generator : ValueGenerator<Guid>
{
    public override bool GeneratesTemporaryValues => false;

    public override Guid Next(EntityEntry entry)
    {
        return Guid.CreateVersion7().ToSqlServerSequential();
    }
}