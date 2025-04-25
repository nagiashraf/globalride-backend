using GlobalRide.Application.Cars.SearchCars;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;
using GlobalRide.Domain.CarTypes;
using GlobalRide.Domain.Common.Result;
using GlobalRide.Domain.Rentals;

using NSubstitute;

using TestsCommon.Branches;
using TestsCommon.Cars;
using TestsCommon.CarTypes;

namespace GlobalRide.Application.UnitTests.Cars;

public class SearchCarsQueryHandlerTests
{
    private readonly SearchCarsQueryHandler _sut;
    private readonly IOneWayRentalService _oneWayServiceMock;
    private readonly IBranchRepository _branchRepositoryMock;
    private readonly ICarRepository _carRepositoryMock;

    public SearchCarsQueryHandlerTests()
    {
        _oneWayServiceMock = Substitute.For<IOneWayRentalService>();

        _branchRepositoryMock = Substitute.For<IBranchRepository>();
        _carRepositoryMock = Substitute.For<ICarRepository>();
        _sut = new SearchCarsQueryHandler(_oneWayServiceMock, _branchRepositoryMock, _carRepositoryMock);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenPickupBranchMissing()
    {
        // Arrange
        var query = new SearchCarsQuery(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now,
            DateTime.Now.AddDays(7));

        _branchRepositoryMock.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Branch?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Errors[0].Type);
    }

    [Fact]
    public async Task Handle_ReturnsValidationError_ForLongRentalPeriod()
    {
        // Arrange
        var query = new SearchCarsQuery(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now,
            DateTime.Now.AddDays(Rental.MaxRentalDays + 1));
        
        _branchRepositoryMock.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(BranchFactory.Create());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Errors[0].Type);
    }

    [Fact]
    public async Task Handle_ReturnsEligibilityErrors_ForInvalidOneWayRental()
    {
        // Arrange
        var query = new SearchCarsQuery(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now,
            DateTime.Now.AddDays(7));
        
        var expectedError = AppError.Validation("Test error");
        _branchRepositoryMock.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(BranchFactory.Create());
        _oneWayServiceMock.IsOneWayRental(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(true);
        _oneWayServiceMock.CheckEligibilityAsync(Arg.Any<Branch>(), Arg.Any<Branch>(), Arg.Any<RentalPeriod>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(expectedError));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedError, result.Errors[0]);
    }

    [Fact]
    public async Task Handle_ThrowsException_ForCarsWithoutCarType()
    {
        // Arrange
        var query = new SearchCarsQuery(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now,
            DateTime.Now.AddDays(7));
        
        _branchRepositoryMock.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(BranchFactory.Create());
        _carRepositoryMock.ListAvailableByBranchAndDatesAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns([CarFactory.Create()]);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _sut.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsCarsWithOneWayFees_ForValidOneWayRental()
    {
        // Arrange
        var query = new SearchCarsQuery(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2025, 1, 1, 0, 0, 0),
            new DateTime(2025, 1, 8, 0, 0, 0));
        
        var branch = BranchFactory.Create();
        var car = CarFactory.Create(carType: CarTypeFactory.Create(multiplier: 1.5m));
        var expectedFee = 100m;

        _branchRepositoryMock.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(branch);
        _carRepositoryMock.ListAvailableByBranchAndDatesAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns([car]);
        _oneWayServiceMock.IsOneWayRental(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(true);
        _oneWayServiceMock.CheckEligibilityAsync(Arg.Any<Branch>(), Arg.Any<Branch>(), Arg.Any<RentalPeriod>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _oneWayServiceMock.CalculateOneWayRentalFee(Arg.Any<Branch>(), Arg.Any<Branch>(), Arg.Any<CarType>(), Arg.Any<DateTime>())
            .Returns(expectedFee);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedFee, result.Value[0].PriceForPeriod - car.DailyRate * 7);
    }

    [Fact]
    public async Task Handle_ReturnsCarsWithoutFees_ForSameBranchRental()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var query = new SearchCarsQuery(
            branchId, branchId,
            new DateTime(2025, 1, 1, 0, 0, 0),
            new DateTime(2025, 1, 8, 0, 0, 0));
        
        var branch = BranchFactory.Create();
        var car = CarFactory.Create(carType: CarTypeFactory.Create(multiplier: 1.5m));

        var expectedPrice = 65.99m * 7m;

        _branchRepositoryMock.GetAsync(branchId, Arg.Any<CancellationToken>()).Returns(branch);
        _carRepositoryMock.ListAvailableByBranchAndDatesAsync(branchId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns([car]);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedPrice, result.Value[0].PriceForPeriod);
    }

    [Fact]
    public async Task Handle_FetchesSingleBranch_ForSamePickupAndDropoff()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var query = new SearchCarsQuery(branchId, branchId, DateTime.Now, DateTime.Now.AddDays(7));

        // Act
        await _sut.Handle(query, CancellationToken.None);

        // Assert
        await _branchRepositoryMock.Received(1)
            .GetAsync(branchId, Arg.Any<CancellationToken>());
    }
}
