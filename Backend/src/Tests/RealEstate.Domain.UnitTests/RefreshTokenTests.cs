using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Identity;
using RealEstate.Domain.Users;
using RealEstate.Infrastructure.Identity;
using RealEstate.Infrastructure.Persistence;
using Xunit;

namespace Tests.RealEstate.Domain.UniTests;

public class RefreshTokenTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;

    public RefreshTokenTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=RealEstateTestDb;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        using var context = new AppDbContext(_options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task RefreshToken_SavesAndLoads_WithCorrectValues()
    {
        var userId = Guid.NewGuid();
        var expectedTokenHash = "test-hash-value-1234567890";
        var expectedExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(1);

        await using (var context = new AppDbContext(_options))
        {
            var applicationUser = new ApplicationUser
            {
                Id = userId,
                UserName = "test@example.com",
                Email = "test@example.com"
            };
            context.Users.Add(applicationUser);
            var domainUser = new User(userId)
            {
                FirstName = "Test",
                LastName = "User"
            };
            context.Set<User>().Add(domainUser);

            await context.SaveChangesAsync();

            var refreshTokenResult = RefreshToken.Create(
                Guid.NewGuid(),
                expectedTokenHash,
                userId,
                expectedExpiresOnUtc);

            Assert.False(refreshTokenResult.IsError);

            context.RefreshTokens.Add(refreshTokenResult.Value);
            await context.SaveChangesAsync();
        }

        await using var readContext = new AppDbContext(_options);
        var loadedToken = await readContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId);

        Assert.NotNull(loadedToken);
        Assert.Equal(expectedTokenHash, loadedToken.TokenHash);
        Assert.Equal(userId, loadedToken.UserId);
        Assert.False(loadedToken.IsRevoked);
        Assert.True(loadedToken.IsActive);
    }

    public void Dispose()
    {
        using var context = new AppDbContext(_options);
        context.Database.EnsureDeleted();
    }
}