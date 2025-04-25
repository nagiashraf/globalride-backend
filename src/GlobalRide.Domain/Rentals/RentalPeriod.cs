namespace GlobalRide.Domain.Rentals;

/// <summary>
/// Represents a rental period with a pickup date and time and a drop-off date and time.
/// </summary>
public record class RentalPeriod
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RentalPeriod"/> class with the specified pickup and dropoff dates and times.
    /// </summary>
    /// <param name="pickup">The pickup date and time in UTC.</param>
    /// <param name="dropoff">The drop-off date and time in UTC.</param>
    public RentalPeriod(DateTime pickup, DateTime dropoff)
    {
        if (pickup >= dropoff)
        {
            throw new ArgumentException("Pickup date and time must be before drop-off date and time.");
        }

        Pickup = pickup;
        Dropoff = dropoff;
    }

    /// <summary>
    /// Gets the pickup date and time in UTC.
    /// </summary>
    public DateTime Pickup { get; init; }

    /// <summary>
    /// Gets the dropoff date and time in UTC.
    /// </summary>
    public DateTime Dropoff { get; init; }

    /// <summary>
    /// Gets the total number of days in the rental period, rounded up to the nearest whole number.
    /// </summary>
    public int Days => (int)Math.Ceiling((Dropoff - Pickup).TotalDays);
}
