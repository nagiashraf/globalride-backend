using GlobalRide.Application.Branches.SearchBranches;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Common;

using NSubstitute;

namespace GlobalRide.Application.UnitTests.Branches;

public class SearchBranchesQueryHandlerTests
{
    private readonly IBranchSearchingService _branchSearchingServiceMock;
    private readonly SearchBranchesQueryHandler _sut;

    public SearchBranchesQueryHandlerTests()
    {
        _branchSearchingServiceMock = Substitute.For<IBranchSearchingService>();
        _sut = new SearchBranchesQueryHandler(_branchSearchingServiceMock);
    }

    [Fact]
    public async Task Handle_ReturnsSuccessWithResults_WhenServiceReturnsData()
    {
        // Arrange
        var request = new SearchBranchesQuery("central", LanguageCode.en, 10);
        var testResults = new List<BranchSearchResultResponse>
        {
            new(Guid.NewGuid(), LanguageCode.en, "Central Hub", "New York", "USA", BranchType.Downtown, "America/New_York"),
            new(Guid.NewGuid(), LanguageCode.en, "London Central", "London", "UK", BranchType.Downtown, "Europe/London")
        };

        _branchSearchingServiceMock.Search(request.SearchTerm, request.LanguageCode, request.MaxResultsCount)
            .Returns(testResults);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testResults.Count, result.Value.Count);
    }

    [Fact]
    public async Task Handle_ReturnsSuccessWithEmptyList_WhenNoResultsFound()
    {
        // Arrange
        var request = new SearchBranchesQuery("invalid", LanguageCode.en, 5);
        _branchSearchingServiceMock.Search(Arg.Any<string>(), Arg.Any<LanguageCode>(), Arg.Any<int>())
            .Returns([]);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}
