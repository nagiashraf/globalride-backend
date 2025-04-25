using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Common;
using GlobalRide.Infrastructure.Persistence;
using GlobalRide.Infrastructure.Persistence.Repositories;

using TestsCommon.Branches;

namespace GlobalRide.Infrastructure.IntegrationTests.Persistence.Repositories;

public class BranchRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
{
    private readonly AppDbContext _context;

    public BranchRepositoryTests(DatabaseFixture fixture)
    {
        Fixture = fixture;
        _context = Fixture.CreateContext();
    }

    public DatabaseFixture Fixture { get; }

    [Fact]
    public async Task GetAsync_ExistingBranch_ReturnsBranch()
    {
        _context.Database.BeginTransaction();

        // Arrange
        var branchId = Guid.NewGuid();
        var branch = BranchFactory.Create(branchId);
        _context.Set<Branch>().Add(branch);
        await _context.SaveChangesAsync();

        var repository = new BranchRepository(_context);

        // Act
        var result = await repository.GetAsync(branchId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(branchId, result.Id);
    }

    [Fact]
    public async Task GetAsync_NonExistentBranch_ReturnsNull()
    {
        _context.Database.BeginTransaction();

        // Arrange (no data seeded)
        var repository = new BranchRepository(_context);

        // Act
        var result = await repository.GetAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ListTranslationsAsync_MultipleBranchesWithTranslations_ReturnsAllTranslations()
    {
        _context.Database.BeginTransaction();

        // Arrange
        var branch1 = BranchFactory.Create(translations:
            [
                new BranchTranslation(LanguageCode: LanguageCode.en, Name: "Downtown", City: "New York", Country: "USA"),
                new BranchTranslation(LanguageCode: LanguageCode.es, Name: "Centro", City: "Nueva York", Country: "EEUU")
            ]);

        var branch2 = BranchFactory.Create(translations:
            [
                new BranchTranslation(LanguageCode: LanguageCode.en, Name: "Westside", City: "Los Angeles", Country: "USA"),
                new BranchTranslation(LanguageCode: LanguageCode.fr, Name: "Cœur Ouest", City: "Los Angeles", Country: "États-Unis")
            ]);

        _context.Set<Branch>().AddRange(branch1, branch2);
        await _context.SaveChangesAsync();

        var repository = new BranchRepository(_context);

        // Act
        var results = await repository.ListTranslationsAsync();

        // Assert
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Id == branch1.Id && r.LanguageCode == LanguageCode.es);
        Assert.Contains(results, r => r.Id == branch2.Id && r.LanguageCode == LanguageCode.fr);
    }

    public void Dispose() => _context.Dispose();
}
