using GlobalRide.Domain.Common.Result;

namespace GlobalRide.Domain.Branches;

/// <summary>
/// Represents a geographic coordinate with latitude and longitude values.
/// </summary>
public record class Coordinate
{
    private Coordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Gets the latitude value in degrees.
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Gets the longitude value in degrees.
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Creates a new <see cref="Coordinate"/> instance after validating the provided latitude and longitude values.
    /// </summary>
    /// <param name="latitude">The latitude value in degrees.</param>
    /// <param name="longitude">The longitude value in degrees.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing either a valid <see cref="Coordinate"/> instance or a list of validation errors.
    /// </returns>
    public static Result<Coordinate> Create(double latitude, double longitude)
    {
        var errors = new List<AppError>();

        if (latitude < -90 || latitude > 90)
        {
            errors.Add(AppError.Validation("Latitude must be between -90 and 90 degrees."));
        }

        if (longitude < -180 || longitude > 180)
        {
            errors.Add(AppError.Validation("Longitude must be between -180 and 180 degrees."));
        }

        return errors.Count > 0
            ? errors
            : new Coordinate(latitude, longitude);
    }

    /// <summary>
    /// Calculates the distance in kilometers between this coordinate and another coordinate using the Haversine formula.
    /// </summary>
    /// <param name="other">The other <see cref="Coordinate"/> to calculate the distance to.</param>
    /// <returns>The distance between the two coordinates in kilometers.</returns>
    public double CalculateDistanceKmTo(Coordinate other)
    {
        const double earthRadiusKm = 6371;

        double latRad = DegreesToRadians(Latitude);
        double lonRad = DegreesToRadians(Longitude);
        double otherLatRad = DegreesToRadians(other.Latitude);
        double otherLonRad = DegreesToRadians(other.Longitude);

        double differenceLat = otherLatRad - latRad;
        double differenceLon = otherLonRad - lonRad;

        double a = Math.Sin(differenceLat / 2) * Math.Sin(differenceLat / 2) +
                   Math.Cos(latRad) * Math.Cos(otherLatRad) *
                   Math.Sin(differenceLon / 2) * Math.Sin(differenceLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distance = earthRadiusKm * c;
        return distance;
    }

    private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180);
}
