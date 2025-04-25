namespace GlobalRide.Domain.Rentals;

/// <summary>
/// Represents the status of a rental.
/// </summary>
public enum RentalStatus
{
    /// <summary>
    /// The rental is pending.
    /// </summary>
    Pending,

    /// <summary>
    /// The rental is confirmed.
    /// </summary>
    Confirmed,

    /// <summary>
    /// The rental is completed.
    /// </summary>
    Completed,

    /// <summary>
    /// The rental is canceled.
    /// </summary>
    Canceled,
}
