using GlobalRide.Domain.Common;

namespace GlobalRide.Domain.Rentals;

/// <summary>
/// Represents a rental entity in the domain.
/// </summary>
public sealed class Rental : Entity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rental"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the rental.</param>
    /// <param name="carId">The unique identifier of the car being rented.</param>
    /// <param name="pickupBranchId">The unique identifier of the branch where the car is picked up.</param>
    /// <param name="dropoffBranchId">The unique identifier of the branch where the car is dropped off.</param>
    /// <param name="period">The rental period, including the pickup and dropoff dates and times.</param>
    /// <param name="pickupDate">The date and time when the car is picked up.</param>
    /// <param name="dropoffDate">The date and time when the car is dropped off.</param>
    /// <param name="totalCost">The total cost of the rental.</param>
    /// <param name="status">The current status of the rental.</param>
    /// <param name="createdAtUtc">The date and time in UTC when the rental was created.</param>
    public Rental(
        Guid id,
        string carId,
        Guid pickupBranchId,
        Guid dropoffBranchId,
        RentalPeriod period,
        decimal totalCost,
        RentalStatus status,
        DateTime createdAtUtc)
        : base(id)
    {
        CarId = carId;
        PickupBranchId = pickupBranchId;
        DropoffBranchId = dropoffBranchId;
        Period = period;
        TotalCost = totalCost;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    private Rental()
    {
    }

    /// <summary>
    /// Gets the maximum number of days allowed for a rental.
    /// </summary>
    public static int MaxRentalDays { get; } = 90;

    /// <summary>
    /// Gets the unique identifier of the car being rented.
    /// </summary>
    public string CarId { get; private set; } = null!;

    /// <summary>
    /// Gets the unique identifier of the branch where the car is picked up.
    /// </summary>
    public Guid PickupBranchId { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the branch where the car is dropped off.
    /// </summary>
    public Guid DropoffBranchId { get; private set; }

/// <summary>
/// Gets the rental period.
/// </summary>
    public RentalPeriod Period { get; private set; } = null!;

    /// <summary>
    /// Gets the total cost of the rental.
    /// </summary>
    public decimal TotalCost { get; private set; }

    /// <summary>
    /// Gets the current status of the rental.
    /// </summary>
    public RentalStatus Status { get; private set; }

    /// <summary>
    /// Gets the date and time in UTC when the rental was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }
}
