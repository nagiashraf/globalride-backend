using GlobalRide.Domain.Branches;

namespace GlobalRide.Domain.UnitTests.Branches;

public class CoordinateTests
{
    [Theory]
    [InlineData(-90, -180)]
    [InlineData(90, 180)]
    [InlineData(0, 0)]
    [InlineData(45.5, -170.3)]
    public void Create_ValidCoordinates_ReturnsSuccessWithCoordinate(double latitude, double longitude)
    {
        // Act
        var result = Coordinate.Create(latitude, longitude);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(latitude, result.Value.Latitude);
        Assert.Equal(longitude, result.Value.Longitude);
    }

    [Theory]
    [InlineData(-91, 0)]
    [InlineData(90.1, 0)]
    [InlineData(100, 0)]
    [InlineData(0, -180.1)]
    [InlineData(0, 180.1)]
    [InlineData(0, 200)]
    public void Create_InvalidLatitudeOrLongitude_ReturnsValidationError(double latitude, double longitude)
    {
        // Act
        var result = Coordinate.Create(latitude, longitude);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Create_InvalidLatitudeAndLongitude_ReturnsBothValidationErrors()
    {
        // Arrange
        double invalidLatitude = -91;
        double invalidLongitude = 190;

        // Act
        var result = Coordinate.Create(invalidLatitude, invalidLongitude);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Count);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0)] // Same point
    [InlineData(0, 0, 0, 1, 111.19)] // 1 degree longitude apart at equator
    [InlineData(0, 0, 10, 0, 1111.95)] // 10 degrees latitude apart
    [InlineData(90, 0, 0, 0, 10007.54)] // North Pole to equator
    [InlineData(0, 0, 0, 180, 20015.09)] // Antipodal points (half circumference)
    public void CalculateDistanceKmTo_KnownValues_ReturnsExpectedDistance(
        double lat1,
        double lon1,
        double lat2,
        double lon2,
        double expectedDistanceKm)
    {
        // Arrange
        var coord1 = Coordinate.Create(lat1, lon1).Value;
        var coord2 = Coordinate.Create(lat2, lon2).Value;

        // Act
        double distance = coord1.CalculateDistanceKmTo(coord2);

        // Assert
        Assert.Equal(expectedDistanceKm, distance, precision: 2); // Allow ±0.01 km variance
    }

    [Fact]
    public void CalculateDistanceKmTo_RealWorldExample_MatchesApproximation()
    {
        // Arrange
        var paris = Coordinate.Create(48.8566, 2.3522).Value;
        var newYork = Coordinate.Create(40.7128, -74.0060).Value;
        double expectedDistanceApproxKm = 5837;

        // Act
        double distance = paris.CalculateDistanceKmTo(newYork);

        // Assert
        Assert.InRange(distance, expectedDistanceApproxKm - 50, expectedDistanceApproxKm + 50);
    }
}
