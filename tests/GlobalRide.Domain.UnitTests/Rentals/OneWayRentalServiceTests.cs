#pragma warning disable SA1204

using GlobalRide.Domain.AllowedCountryPairs;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;
using GlobalRide.Domain.CarTypes;
using GlobalRide.Domain.Rentals;

using NSubstitute;

namespace GlobalRide.Domain.UnitTests.Rentals;

public class OneWayRentalServiceTests
{
    private readonly IAllowedCountryPairRepository _allowedCountryPairRepositoryMock = Substitute.For<IAllowedCountryPairRepository>();
    private readonly ICarRepository _carRepositoryMock = Substitute.For<ICarRepository>();
    private readonly OneWayRentalService _sut;

    public OneWayRentalServiceTests()
    {
        _sut = new OneWayRentalService(_allowedCountryPairRepositoryMock, _carRepositoryMock);
    }

    [Fact]
    public void IsOneWayRental_ReturnsTrue_WhenBranchesAreDifferent()
    {
        // Arrange
        var pickupBranchId = Guid.NewGuid();
        var dropoffBranchId = Guid.NewGuid();

        // Act
        var result = _sut.IsOneWayRental(pickupBranchId, dropoffBranchId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsOneWayRental_ReturnsFalse_WhenBranchesAreSame()
    {
        // Arrange
        var branchId = Guid.NewGuid();

        // Act
        var result = _sut.IsOneWayRental(branchId, branchId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CheckEligibilityAsync_ValidParameters_ReturnsSuccess()
    {
        // Arrange
        var pickupBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "US",
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: true,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var dropoffBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "US",
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: true,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var pickupDate = DateTime.Now;
        var dropoffDate = pickupDate.AddDays(3);
        var period = new RentalPeriod(pickupDate, dropoffDate);

        _carRepositoryMock.CountByBranchIdAsync(dropoffBranch.Id, Arg.Any<CancellationToken>()).Returns(5);
        _carRepositoryMock.CountIncomingByBranchIdAsync(dropoffBranch.Id, period.Dropoff, Arg.Any<CancellationToken>()).Returns(2);

        // Act
        var result = await _sut.CheckEligibilityAsync(pickupBranch, dropoffBranch, period, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CheckEligibilityAsync_DropoffBranchCapacityExceeded_ReturnsFailure()
    {
        // Arrange
        var pickupBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "US",
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: true,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var dropoffBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "US",
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: true,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var pickupDate = DateTime.Now;
        var dropoffDate = pickupDate.AddDays(3);
        var period = new RentalPeriod(pickupDate, dropoffDate);

        _carRepositoryMock.CountByBranchIdAsync(dropoffBranch.Id, Arg.Any<CancellationToken>()).Returns(8);
        _carRepositoryMock.CountIncomingByBranchIdAsync(dropoffBranch.Id, period.Dropoff, Arg.Any<CancellationToken>()).Returns(3);

        // Act
        var result = await _sut.CheckEligibilityAsync(pickupBranch, dropoffBranch, period, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == RentalErrors.DropoffBranchCapacityExceeded.Code);
    }

    [Fact]
    public async Task CheckEligibilityAsync_DropoffBranchNotAllowed_ReturnsFailure()
    {
        // Arrange
        var pickupBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "US",
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: true,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var dropoffBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "US",
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: false,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var pickupDate = DateTime.Now;
        var dropoffDate = pickupDate.AddDays(3);
        var period = new RentalPeriod(pickupDate, dropoffDate);

        _carRepositoryMock.CountByBranchIdAsync(dropoffBranch.Id, Arg.Any<CancellationToken>()).Returns(5);
        _carRepositoryMock.CountIncomingByBranchIdAsync(dropoffBranch.Id, period.Dropoff, Arg.Any<CancellationToken>()).Returns(2);

        // Act
        var result = await _sut.CheckEligibilityAsync(pickupBranch, dropoffBranch, period, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == RentalErrors.DropoffBranchNotAllowed.Code);
    }

    [Fact]
    public async Task CheckEligibilityAsync_CountryPairNotAllowed_ReturnsFailure()
    {
        // Arrange
        var pickupBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "US",
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: true,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var dropoffBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "CA",
            timeZone: "Canada/Torronto",
            capacity: 10,
            isOneWayDropoffAllowed: true,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var pickupDate = DateTime.Now;
        var dropoffDate = pickupDate.AddDays(3);
        var period = new RentalPeriod(pickupDate, dropoffDate);

        _carRepositoryMock.CountByBranchIdAsync(dropoffBranch.Id, Arg.Any<CancellationToken>()).Returns(5);
        _carRepositoryMock.CountIncomingByBranchIdAsync(dropoffBranch.Id, period.Dropoff, Arg.Any<CancellationToken>()).Returns(2);
        _allowedCountryPairRepositoryMock.ExistsAsync("US", "CA", Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.CheckEligibilityAsync(pickupBranch, dropoffBranch, period, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == RentalErrors.CountryPairNotAllowed.Code);
    }

    [Fact]
    public async Task CheckEligibilityAsync_MultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var pickupBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "US",
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: false,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var dropoffBranch = new Branch(
            id: Guid.NewGuid(),
            countryCode: "CA",
            timeZone: "Canada/Torronto",
            capacity: 10,
            isOneWayDropoffAllowed: false,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(0, 0).Value,
            translations: []);

        var pickupDate = DateTime.Now;
        var dropoffDate = pickupDate.AddHours(12);
        var period = new RentalPeriod(pickupDate, dropoffDate);

        _carRepositoryMock.CountByBranchIdAsync(dropoffBranch.Id, Arg.Any<CancellationToken>()).Returns(5);
        _carRepositoryMock.CountIncomingByBranchIdAsync(dropoffBranch.Id, period.Dropoff, Arg.Any<CancellationToken>()).Returns(2);
        _allowedCountryPairRepositoryMock.ExistsAsync("US", "CA", Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.CheckEligibilityAsync(pickupBranch, dropoffBranch, period, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == RentalErrors.DropoffBranchNotAllowed.Code);
        Assert.Contains(result.Errors, e => e.Code == RentalErrors.CountryPairNotAllowed.Code);
    }

    [Fact]
    public void CalculateOneWayRentalFee_IncludesMinimumFlatDistanceFee()
    {
        // Arrange
        var pickupBranch = TestHelpers.CreateBranch("US", 0, 0);
        var dropoffBranch = TestHelpers.CreateBranch("US", 0, 0);
        var carType = TestHelpers.CreateCarType(1.0m);
        var pickupDate = new DateTime(2025, 6, 1, 0, 0, 0);
        var minimumDistanceFee = 30m;

        // Act
        var fee = _sut.CalculateOneWayRentalFee(pickupBranch, dropoffBranch, carType, pickupDate);

        // Assert
        Assert.Equal(minimumDistanceFee, fee);
    }

    [Fact]
    public void CalculateOneWayRentalFee_AddsPerKilometerRateToDistanceFee()
    {
        // Arrange
        var pickupBranch = TestHelpers.CreateBranch("US", 0, 0);
        var dropoffBranch = TestHelpers.CreateBranch("US", 0, 1); // ~111 km at equator
        var carType = TestHelpers.CreateCarType(1.0m);
        var pickupDate = new DateTime(2025, 6, 1, 0, 0, 0);

        // Calculate expected values
        var distanceKm = pickupBranch.Coordinate.CalculateDistanceKmTo(dropoffBranch.Coordinate);
        var expectedFee = 30m + (decimal)distanceKm * 0.2m;

        // Act
        var fee = _sut.CalculateOneWayRentalFee(pickupBranch, dropoffBranch, carType, pickupDate);

        // Assert
        Assert.Equal(expectedFee, fee, precision: 1);
    }

    [Fact]
    public void CalculateOneWayRentalFee_AddsCrossBorderFee_WhenBranchesInSameCountry()
    {
        // Arrange
        var pickupBranch = TestHelpers.CreateBranch("US", 0, 0);
        var dropoffBranch = TestHelpers.CreateBranch("US", 0, 0);
        var carType = TestHelpers.CreateCarType(1.0m);
        var pickupDate = new DateTime(2025, 6, 1, 0, 0, 0);
        var expectedFee = 30m; // 30 (distance)

        // Act
        var fee = _sut.CalculateOneWayRentalFee(pickupBranch, dropoffBranch, carType, pickupDate);

        // Assert
        Assert.Equal(expectedFee, fee);
    }

    [Fact]
    public void CalculateOneWayRentalFee_AddsCrossBorderFee_WhenBranchesInDifferentCountries()
    {
        // Arrange
        var pickupBranch = TestHelpers.CreateBranch("US", 0, 0);
        var dropoffBranch = TestHelpers.CreateBranch("CA", 0, 0);
        var carType = TestHelpers.CreateCarType(1.0m);
        var pickupDate = new DateTime(2023, 6, 1, 0, 0, 0);
        var expectedFee = 30m + 100m; // 30 (distance) + 100 (cross-border)

        // Act
        var fee = _sut.CalculateOneWayRentalFee(pickupBranch, dropoffBranch, carType, pickupDate);

        // Assert
        Assert.Equal(expectedFee, fee);
    }

    [Fact]
    public void CalculateOneWayRentalFee_AppliesSeasonalFactorDuringPeakSeason()
    {
        // Arrange
        var pickupBranch = TestHelpers.CreateBranch("US", 0, 0);
        var dropoffBranch = TestHelpers.CreateBranch("US", 0, 0);
        var carType = TestHelpers.CreateCarType(1.0m);
        var pickupDate = new DateTime(2024, 12, 25, 0, 0, 0);
        var expectedFee = 30m * 1.3m; // 30 (distance) * 1.3 (seasonal factor)

        // Act
        var fee = _sut.CalculateOneWayRentalFee(pickupBranch, dropoffBranch, carType, pickupDate);

        // Assert
        Assert.Equal(expectedFee, fee);
    }

    [Fact]
    public void CalculateOneWayRentalFee_ThrowsWhenCarTypeMultiplierIsNull()
    {
        // Arrange
        var pickupBranch = TestHelpers.CreateBranch("US", 0, 0);
        var dropoffBranch = TestHelpers.CreateBranch("US", 0, 0);
        var carType = CarType.Create(Guid.NewGuid(), "SUV", false, null).Value;
        var pickupDate = new DateTime(2025, 6, 1, 0, 0, 0);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sut.CalculateOneWayRentalFee(pickupBranch, dropoffBranch, carType, pickupDate));
    }

    [Fact]
    public void CalculateOneWayRentalFee_AppliesCarTypeMultiplier()
    {
        // Arrange
        var pickupBranch = TestHelpers.CreateBranch("US", 0, 0);
        var dropoffBranch = TestHelpers.CreateBranch("US", 0, 0);
        var carType = TestHelpers.CreateCarType(1.5m);
        var pickupDate = new DateTime(2025, 6, 1, 0, 0, 0);
        var expectedFee = 30m * 1.5m; // 30 (distance) * 1.5 (car type multiplier)

        // Act
        var fee = _sut.CalculateOneWayRentalFee(pickupBranch, dropoffBranch, carType, pickupDate);

        // Assert
        Assert.Equal(expectedFee, fee);
    }

    [Fact]
    public void CalculateOneWayRentalFee_CombinesAllFactorsCorrectly()
    {
        // Arrange
        var pickupBranch = TestHelpers.CreateBranch("US", 0, 0);
        var dropoffBranch = TestHelpers.CreateBranch("CA", 0, 1); // ~111 km distance
        var carType = TestHelpers.CreateCarType(2.0m);
        var pickupDate = new DateTime(2025, 12, 31, 0, 0, 0);

        // Calculate expected values
        var distanceKm = pickupBranch.Coordinate.CalculateDistanceKmTo(dropoffBranch.Coordinate);
        var distanceFee = 30m + (decimal)distanceKm * 0.2m;
        var expectedFee = (distanceFee + 100m) * 2.0m * 1.3m;

        // Act
        var fee = _sut.CalculateOneWayRentalFee(pickupBranch, dropoffBranch, carType, pickupDate);

        // Assert
        Assert.Equal(expectedFee, fee, precision: 1);
    }
}

public static class TestHelpers
{
    public static Branch CreateBranch(string countryCode, double lat, double lon) =>
        new(
            id: Guid.NewGuid(),
            countryCode: countryCode,
            timeZone: "America/New_York",
            capacity: 10,
            isOneWayDropoffAllowed: false,
            type: BranchType.Airport,
            coordinate: Coordinate.Create(lat, lon).Value,
            translations: []);

    public static CarType CreateCarType(decimal multiplier) =>
        CarType.Create(
            Guid.NewGuid(),
            "SUV",
            true,
            multiplier).Value;
}
