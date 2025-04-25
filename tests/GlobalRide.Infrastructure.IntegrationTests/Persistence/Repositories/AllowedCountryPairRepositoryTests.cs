using GlobalRide.Domain.AllowedCountryPairs;
using GlobalRide.Infrastructure.Persistence;
using GlobalRide.Infrastructure.Persistence.Repositories;

namespace GlobalRide.Infrastructure.IntegrationTests.Persistence.Repositories;

public class AllowedCountryPairRepositoryTests  : IClassFixture<DatabaseFixture>, IDisposable
{
    private readonly AppDbContext _context;

    public AllowedCountryPairRepositoryTests(DatabaseFixture fixture)
    {
        Fixture = fixture;
        _context = Fixture.CreateContext();
    }

    public DatabaseFixture Fixture { get; }

    [Fact]
    public async Task ExistsAsync_WithMultiplePairs_ReturnsCorrectResultForEachPair()
    {
        _context.Database.BeginTransaction();

        // Arrange
        var allowedPairs = new[]
        {
            new AllowedCountryPair("US", "CA"),
            new AllowedCountryPair("FR", "DE"),
            new AllowedCountryPair("JP", "KR"),
        };
        _context.Set<AllowedCountryPair>().AddRange(allowedPairs);
        await _context.SaveChangesAsync();

        var repository = new AllowedCountryPairRepository(_context);

        // Act
        bool usCaExists = await repository.ExistsAsync("US", "CA");
        bool frDeExists = await repository.ExistsAsync("FR", "DE");
        bool jpKrExists = await repository.ExistsAsync("JP", "KR");
        bool usFrExists = await repository.ExistsAsync("US", "FR"); // Not allowed

        // Assert
        Assert.True(usCaExists);
        Assert.True(frDeExists);
        Assert.True(jpKrExists);
        Assert.False(usFrExists);
    }

    public void Dispose() => _context.Dispose();
}
