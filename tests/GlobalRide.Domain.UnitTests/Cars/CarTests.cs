using GlobalRide.Domain.Cars;
using GlobalRide.Domain.Rentals;

namespace GlobalRide.Domain.UnitTests.Cars;

public class CarTests
{
    [Theory]
    [InlineData(1, 0, 65.99)] // 1 day, no fee
    [InlineData(3, 50.0, 247.97)] // 3 days + fee (65.99*3 + 50)
    public void CalculateTotalPrice_ValidParameters_ReturnsCorrectTotal(
        int rentalDays,
        decimal oneWayFee,
        decimal expectedTotal)
    {
        // Arrange
        var car = CreateTestCar();
        var period = CreateRentalPeriod(rentalDays);

        // Act
        var totalPrice = car.CalculateTotalPrice(period, oneWayFee);

        // Assert
        Assert.Equal(expectedTotal, totalPrice, precision: 2);
    }

    [Fact]
    public void CalculateTotalPrice_WithoutOptionalFee_DefaultsToZero()
    {
        // Arrange
        var car = CreateTestCar();
        var period = CreateRentalPeriod(5);

        // Act
        var totalPrice = car.CalculateTotalPrice(period);

        // Assert
        Assert.Equal(329.95m, totalPrice); // 65.99 * 5
    }

    private static Car CreateTestCar() =>
        new(
            id: "1HGCM82633A123456",
            branchId: Guid.NewGuid(),
            carTypeId: Guid.NewGuid(),
            make: "Honda",
            model: "Accord",
            year: 2025,
            seatsCount: 5,
            dailyRate: 65.99m,
            transmissionType: TransmissionType.Automatic,
            fuelType: FuelType.Gasoline,
            null);

    private static RentalPeriod CreateRentalPeriod(int days)
    {
        var pickup = new DateTime(2025, 1, 1, 8, 0, 0);
        var dropoff = pickup.AddDays(days);
        return new RentalPeriod(pickup, dropoff);
    }
}
