using GlobalRide.Domain.Rentals;

namespace GlobalRide.Domain.UnitTests.Rentals;

public class RentalPeriodTests
{
    public static TheoryData<DateTimeOffset, DateTimeOffset, int> RentalPeriodTestCases =>
        new()
        {
            // 23h59m59s duration
            {
                new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 1, 23, 59, 59, TimeSpan.Zero),
                1
            },
            // Exactly 24 hours
            {
                new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero),
                1
            },
            // 24h00m01s duration
            {
                new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 0, 0, 1, TimeSpan.Zero),
                2
            },
            // 25 hours duration
            {
                new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 1, 0, 0, TimeSpan.Zero),
                2
            },
        };

    [Theory]
    [MemberData(nameof(RentalPeriodTestCases))]
    public void Days_CalculatesCorrectly(
        DateTime pickup,
        DateTime dropoff,
        int expectedDays)
    {
        // Arrange
        var rentalPeriod = new RentalPeriod(pickup, dropoff);

        // Act
        var actualDays = rentalPeriod.Days;

        // Assert
        Assert.Equal(expectedDays, actualDays);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenPickupIsSameAsDropoff()
    {
        // Arrange
        var pickup = DateTime.Now;
        var dropoff = pickup; // Same as pickup

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RentalPeriod(pickup, dropoff));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenPickupIsAfterDropoff()
    {
        // Arrange
        var pickup = DateTime.Now;
        var dropoff = pickup.AddDays(-1); // Dropoff before pickup

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RentalPeriod(pickup, dropoff));
    }
}
