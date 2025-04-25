using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Common;
using GlobalRide.Infrastructure.FullTextSearch.LuceneNet;

namespace GlobalRide.Infrastructure.IntegrationTests.FullTextSearch;

public class LuceneBranchSearchingServiceTests : IDisposable
{
    private readonly string _indexDirectoryPath;
    private readonly LuceneBranchSearchingService _service;

    public LuceneBranchSearchingServiceTests()
    {
        // Create a unique temporary directory for each test
        _indexDirectoryPath = Path.Combine(Path.GetTempPath(), "LuceneBranchSearchingServiceTests" + Guid.NewGuid().ToString());
        _service = new LuceneBranchSearchingService(_indexDirectoryPath);
    }

    public void Dispose()
    {
        _service.Dispose();
        if (System.IO.Directory.Exists(_indexDirectoryPath))
            System.IO.Directory.Delete(_indexDirectoryPath, recursive: true);
    }

    [Fact]
    public void AddRange_SingleBranch_CanBeFoundByExactNameSearch()
    {
        // Arrange
        var branch = new BranchSearchResultResponse(
            Id: Guid.NewGuid(),
            LanguageCode: LanguageCode.en,
            Name: "Central Station",
            City: "Berlin",
            Country: "Germany",
            Type: BranchType.Downtown,
            TimeZone: "Europe/Berlin");

        // Act
        _service.AddRange([branch]);

        // Assert
        var results = _service.Search("Central", LanguageCode.en, 10);

        Assert.Single(results);
        Assert.Equal(branch.Name, results[0].Name);
    }

    [Fact]
    public void AddRange_BranchWithDiacritics_CanBeFoundByNormalizedTerm()
    {
        // Arrange
        var branch = new BranchSearchResultResponse(
            Id: Guid.NewGuid(),
            LanguageCode: LanguageCode.fr,
            Name: "Aéroport de Tambacounda",
            City: "Tambacounda",
            Country: "Sénégal",
            Type: BranchType.Airport,
            TimeZone: "Africa/Tambacounda");

        // Act
        _service.AddRange([branch]);

        // Assert
        var results = _service.Search("senegal", LanguageCode.fr, 10);

        Assert.Single(results);
    }

    [Fact]
    public void AddRange_BranchWithUppercaseName_CanBeFoundByLowercaseSearch()
    {
        // Arrange
        var branch = new BranchSearchResultResponse(
            Id: Guid.NewGuid(),
            LanguageCode: LanguageCode.en,
            Name: "HEATHROW AIRPORT",
            City: "London",
            Country: "UK",
            Type: BranchType.Airport,
            TimeZone: "Europe/London");

        // Act
        _service.AddRange([branch]);

        // Assert
        var results = _service.Search("heathrow", LanguageCode.en, 10); // Lowercase search term

        Assert.Single(results);
    }

    [Fact]
    public void Search_ExactMatchInAllLanguages_ReturnsBoostedResultsFirst()
    {
        // Arrange
        var branches = new[]
        {
            new BranchSearchResultResponse(
                Id: Guid.NewGuid(),
                LanguageCode: LanguageCode.fr,
                Name: "Gare de Lyon",
                City: "Lyon",
                Country: "France",
                Type: BranchType.Downtown,
                TimeZone: "Europe/Lyon"),
            new BranchSearchResultResponse(
                Id: Guid.NewGuid(),
                LanguageCode: LanguageCode.es,
                Name: "Estación de Lyon",
                City: "Lyon",
                Country: "Francia",
                Type: BranchType.Downtown,
                TimeZone: "Europe/Lyon"),
            new BranchSearchResultResponse(
                Id: Guid.NewGuid(),
                LanguageCode: LanguageCode.en,
                Name: "Lyon Station",
                City: "Lyon",
                Country: "France",
                Type: BranchType.Downtown,
                TimeZone: "Europe/Lyon")
        };

        _service.AddRange(branches);

        // Act (French is preferred language)
        var results = _service.Search("lyon", LanguageCode.fr, 10);

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal(LanguageCode.fr, results[0].LanguageCode); // French result boosted
        Assert.Equal(LanguageCode.en, results[1].LanguageCode); // English boosted as a fallback over Spanish
    }

    [Fact]
    public void Search_CityFieldBoostedHigherThanName()
    {
        // Arrange
        var branches = new[]
        {
            new BranchSearchResultResponse(
                Id: Guid.NewGuid(),
                LanguageCode: LanguageCode.en,
                Name: "Berlin Central",
                City: "Munich",
                Country: "Germany",
                Type: BranchType.Downtown,
                TimeZone: "Europe/Munich"),
            new BranchSearchResultResponse(
                Id: Guid.NewGuid(),
                LanguageCode: LanguageCode.en,
                Name: "Munich Station",
                City: "Berlin",
                Country: "Germany",
                Type: BranchType.Downtown,
                TimeZone: "Europe/Berlin")
        };

        _service.AddRange(branches);

        // Act
        var results = _service.Search("berlin", LanguageCode.en, 10);

        // Assert: City match ("Berlin") should rank higher than Name match ("Berlin Central")
        Assert.Equal("Berlin", results[0].City);
    }

    [Fact]
    public void Search_FuzzyMatching_ReturnsMisspelledResult()
    {
        // Arrange
        var branch = new BranchSearchResultResponse(
            Id: Guid.NewGuid(),
            LanguageCode: LanguageCode.en,
            Name: "Heathrow Airport",
            City: "London",
            Country: "UK",
            Type: BranchType.Airport,
            TimeZone: "Europe/London");

        _service.AddRange([branch]);

        // Act
        var results = _service.Search("heathro", LanguageCode.en, 10); // Missing 'w'

        // Assert
        Assert.Single(results);
        Assert.Equal("Heathrow Airport", results[0].Name);
    }

    [Fact]
    public void Search_EmptySearchTerm_ReturnsEmptyList()
    {
        // Arrange
        _service.AddRange(
        [
            new BranchSearchResultResponse(
                Id: Guid.NewGuid(),
                LanguageCode: LanguageCode.en,
                Name: "Test Branch",
                City: "Test City",
                Country: "Test Country",
                Type: BranchType.Downtown,
                TimeZone: "Test TimeZone")
        ]);

        // Act
        var results = _service.Search("   ", LanguageCode.en, 10);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Search_MaxResultsLimit_ReturnsOnlyRequestedCount()
    {
        // Arrange
        var branches = new List<BranchSearchResultResponse>();
        for (int i = 0; i < 20; i++)
        {
            branches.Add(new BranchSearchResultResponse(
                Id: Guid.NewGuid(),
                LanguageCode: LanguageCode.en,
                Name: $"Branch {i}",
                City: "City",
                Country: "Country",
                Type: BranchType.Downtown,
                TimeZone: "TimeZone"));
        }

        _service.AddRange(branches);

        // Act
        var results = _service.Search("branch", LanguageCode.en, 5);

        // Assert
        Assert.Equal(5, results.Count);
    }
}
