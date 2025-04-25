using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;
using GlobalRide.Domain.CarTypes;
using GlobalRide.Domain.Rentals;
using GlobalRide.Infrastructure.Persistence;
using GlobalRide.Infrastructure.Persistence.Repositories;

using TestsCommon.Branches;
using TestsCommon.Cars;
using TestsCommon.CarTypes;
using TestsCommon.Rentals;

namespace GlobalRide.Infrastructure.IntegrationTests.Persistence.Repositories;

public class CarRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
{
    private readonly Guid _branchId;
    private readonly Guid _carTypeId;
    private readonly string _carId;
    private readonly AppDbContext _context;

    public CarRepositoryTests(DatabaseFixture fixture)
    {
        Fixture = fixture;
        _branchId = Guid.NewGuid();
        _carTypeId = Guid.NewGuid();
        _carId = "12345678901234567";
        _context = Fixture.CreateContext();
    }

    public DatabaseFixture Fixture { get; }

    [Fact]
    public async Task ListAvailableByBranchAndDatesAsync_NoConflictingRentals_ReturnsAllCarsInBranch()
    {
        _context.Database.BeginTransaction();

        // Arrange
        _context.Set<Branch>().Add(BranchFactory.Create(_branchId));
        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));

        var car1 = CarFactory.Create(
            branchId: _branchId,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create());

        var car2 = CarFactory.Create(
            branchId: _branchId,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create());

        _context.Set<Car>().AddRange(car1, car2);
        await _context.SaveChangesAsync();

        var repository = new CarRepository(_context);

        // Act
        var result = await repository.ListAvailableByBranchAndDatesAsync(
            _branchId, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == car1.Id);
        Assert.Contains(result, c => c.Id == car2.Id);
    }

    [Fact]
    public async Task ListAvailableByBranchAndDatesAsync_WithConflictingRental_ExcludesCar()
    {
        _context.Database.BeginTransaction();

        // Arrange
        _context.Set<Branch>().Add(BranchFactory.Create(_branchId));
        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));
        _context.Set<Car>().Add(CarFactory.Create(vin: _carId, branchId: _branchId, carTypeId: _carTypeId, carType: CarTypeFactory.Create()));

        var rental = RentalFactory.Create(
            carId: _carId,
            pickupBranchId: _branchId,
            dropoffBranchId: _branchId,
            pickupDate: DateTime.UtcNow.AddDays(-1),
            dropoffDate: DateTime.UtcNow.AddDays(2));

        _context.Set<Rental>().Add(rental);
        await _context.SaveChangesAsync();

        var repository = new CarRepository(_context);

        var pickupDate = DateTime.UtcNow;
        var dropoffDate = pickupDate.AddDays(1);

        // Act
        var result = await repository.ListAvailableByBranchAndDatesAsync(_branchId, pickupDate, dropoffDate);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListAvailableByBranchAndDatesAsync_CarsInDifferentBranch_ExcludesThem()
    {
        _context.Database.BeginTransaction();

        // Arrange
        var branch1Id = Guid.NewGuid();
        var branch2Id = Guid.NewGuid();

        _context.Set<Branch>().AddRange(BranchFactory.Create(branch1Id), BranchFactory.Create(branch2Id));
        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));

        var carInBranch1 = CarFactory.Create(
            branchId: branch1Id,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create());

        var carInBranch2 = CarFactory.Create(
            branchId: branch2Id,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create());

        _context.Set<Car>().AddRange(carInBranch1, carInBranch2);
        await _context.SaveChangesAsync();

        var repository = new CarRepository(_context);

        // Act
        var result = await repository.ListAvailableByBranchAndDatesAsync(
            branch1Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Assert
        Assert.Single(result);
        Assert.Equal(carInBranch1.Id, result[0].Id);
    }

    [Fact]
    public async Task ListAvailableByBranchAndDatesAsync_RentalEndsOnPickupDate_ExcludesCar()
    {
        _context.Database.BeginTransaction();

        // Arrange
        _context.Set<Branch>().Add(BranchFactory.Create(_branchId));
        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));
        _context.Set<Car>().Add(CarFactory.Create(
            vin: _carId,
            branchId: _branchId,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create()));

        var pickupDate = new DateTime(2025, 1, 1, 0, 0, 0);
        var dropoffDate = new DateTime(2025, 1, 2, 0, 0, 0);

        var rental = RentalFactory.Create(
            carId: _carId,
            pickupBranchId: _branchId,
            dropoffBranchId: _branchId,
            pickupDate: pickupDate.AddDays(-2),
            dropoffDate: pickupDate); // Ends exactly at pickupDate

        _context.Set<Rental>().Add(rental);
        await _context.SaveChangesAsync();

        var repository = new CarRepository(_context);

        // Act
        var result = await repository.ListAvailableByBranchAndDatesAsync(_branchId, pickupDate, dropoffDate);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListAvailableByBranchAndDatesAsync_RentalStartsOnDropoffDate_ExcludesCar()
    {
        _context.Database.BeginTransaction();

        // Arrange
        _context.Set<Branch>().Add(BranchFactory.Create(_branchId));
        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));
        _context.Set<Car>().Add(CarFactory.Create(
            vin: _carId,
            branchId: _branchId,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create()));

        var pickupDate = new DateTime(2025, 1, 1, 0, 0, 0);
        var dropoffDate = new DateTime(2025, 1, 2, 0, 0, 0);

        var rental = RentalFactory.Create(
            carId: _carId,
            pickupBranchId: _branchId,
            dropoffBranchId: _branchId,
            pickupDate: dropoffDate, // Starts on dropoffDate
            dropoffDate: dropoffDate.AddDays(1));


        _context.Set<Rental>().Add(rental);
        await _context.SaveChangesAsync();

        var repository = new CarRepository(_context);

        // Act
        var result = await repository.ListAvailableByBranchAndDatesAsync(_branchId, pickupDate, dropoffDate);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CountByBranchIdAsync_WithCarsInBranch_ReturnsCorrectCount()
    {
        // Arrange
        _context.Database.BeginTransaction();

        var branch1Id = Guid.NewGuid();
        var branch2Id = Guid.NewGuid();

        _context.Set<Branch>().AddRange(
            BranchFactory.Create(branch1Id), 
            BranchFactory.Create(branch2Id)
        );

        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));

        // Add 2 cars to Branch 1
        for (int i = 0; i < 2; i++)
        {
            _context.Set<Car>().Add(CarFactory.Create(
                branchId: branch1Id,
                carTypeId: _carTypeId,
                carType: CarTypeFactory.Create()));
        }

        // Add 3 cars to Branch 2
        for (int i = 0; i < 3; i++)
        {
            _context.Set<Car>().Add(CarFactory.Create(
                branchId: branch2Id,
                carTypeId: _carTypeId,
                carType: CarTypeFactory.Create()));
        }

        await _context.SaveChangesAsync();

        var repository = new CarRepository(_context);

        // Act
        int branch1Count = await repository.CountByBranchIdAsync(branch1Id);
        int branch2Count = await repository.CountByBranchIdAsync(branch2Id);

        // Assert
        Assert.Equal(2, branch1Count);
        Assert.Equal(3, branch2Count);
    }

    [Fact]
    public async Task CountByBranchIdAsync_NoCarsInBranch_ReturnsZero()
    {
        // Arrange
        _context.Database.BeginTransaction();

        _context.Set<Branch>().Add(BranchFactory.Create(_branchId));

        await _context.SaveChangesAsync(); // No cars added

        var repository = new CarRepository(_context);

        // Act
        int result = await repository.CountByBranchIdAsync(_branchId);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountIncomingByBranchIdAsync_WithValidIncomingRentals_ReturnsCorrectCount()
    {
        // Arrange
        _context.Database.BeginTransaction();

        var targetBranchId = Guid.NewGuid();
        var pickupBranchId = Guid.NewGuid();
        var date = new DateTime(2025, 10, 5, 0, 0, 0);

        _context.Set<Branch>().AddRange(
            BranchFactory.Create(targetBranchId),
            BranchFactory.Create(pickupBranchId)
        );

        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));

        _context.Set<Car>().Add(CarFactory.Create(
            vin: _carId,
            branchId: pickupBranchId,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create()));

        // Valid incoming rental (not canceled, dropoff on target date)
        _context.Set<Rental>().Add(RentalFactory.Create(
            carId: _carId,
            pickupBranchId: pickupBranchId,
            dropoffBranchId: targetBranchId,
            pickupDate: date.AddDays(-1),
            dropoffDate: date.AddHours(12),
            status: RentalStatus.Confirmed));

        await _context.SaveChangesAsync();
        var repository = new CarRepository(_context);

        // Act
        int result = await repository.CountIncomingByBranchIdAsync(targetBranchId, date);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CountIncomingByBranchIdAsync_CanceledRental_ExcludedFromCount()
    {
        // Arrange
        _context.Database.BeginTransaction();

        var targetBranchId = Guid.NewGuid();
        var pickupBranchId = Guid.NewGuid();
        var date = new DateTime(2025, 10, 5, 0, 0, 0);

        _context.Set<Branch>().AddRange(
            BranchFactory.Create(targetBranchId),
            BranchFactory.Create(pickupBranchId)
        );

        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));

        _context.Set<Car>().Add(CarFactory.Create(
            vin: _carId,
            branchId: pickupBranchId,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create()));

        // Valid rental but marked as canceled
        _context.Set<Rental>().Add(RentalFactory.Create(
            carId: _carId,
            pickupBranchId: pickupBranchId,
            dropoffBranchId: targetBranchId,
            pickupDate: date.AddDays(-1),
            dropoffDate: date.AddHours(12),
            status: RentalStatus.Canceled));

        await _context.SaveChangesAsync();

        var repository = new CarRepository(_context);

        // Act
        int result = await repository.CountIncomingByBranchIdAsync(targetBranchId, date);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountIncomingByBranchIdAsync_SamePickupAndDropoffBranch_ExcludedFromCount()
    {
        // Arrange
        _context.Database.BeginTransaction();

        var date = new DateTime(2025, 10, 5, 0, 0, 0);

        _context.Set<Branch>().AddRange(BranchFactory.Create(_branchId));

        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));

        _context.Set<Car>().Add(CarFactory.Create(
            vin: _carId,
            branchId: _branchId,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create()));

        _context.Set<Rental>().Add(RentalFactory.Create(
            carId: _carId,
            pickupBranchId: _branchId, // Same as dropoff branch
            dropoffBranchId: _branchId,
            pickupDate: date.AddDays(-1),
            dropoffDate: date.AddHours(12),
            status: RentalStatus.Confirmed));

        await _context.SaveChangesAsync();
        var repository = new CarRepository(_context);

        // Act
        int result = await repository.CountIncomingByBranchIdAsync(_branchId, date);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountIncomingByBranchIdAsync_DropoffAtMidnightNextDay_ExcludedFromCount()
    {
        // Arrange
        _context.Database.BeginTransaction();

        var targetBranchId = Guid.NewGuid();
        var pickupBranchId = Guid.NewGuid();
        var date = new DateTime(2025, 10, 5, 0, 0, 0);

        _context.Set<Branch>().AddRange(
            BranchFactory.Create(targetBranchId),
            BranchFactory.Create(pickupBranchId)
        );

        _context.Set<CarType>().Add(CarTypeFactory.Create(_carTypeId));

        _context.Set<Car>().Add(CarFactory.Create(
            vin: _carId,
            branchId: pickupBranchId,
            carTypeId: _carTypeId,
            carType: CarTypeFactory.Create()));

        // Dropoff at midnight of the next day (excluded)
        _context.Set<Rental>().Add(RentalFactory.Create(
            carId: _carId,
            pickupBranchId: pickupBranchId, // Same as dropoff branch
            dropoffBranchId: targetBranchId,
            pickupDate: date,
            dropoffDate: date.AddDays(1),
            status: RentalStatus.Confirmed));

        await _context.SaveChangesAsync();
        var repository = new CarRepository(_context);

        // Act
        int result = await repository.CountIncomingByBranchIdAsync(targetBranchId, date);

        // Assert
        Assert.Equal(0, result);
    }

    public void Dispose() => _context.Dispose();
}
