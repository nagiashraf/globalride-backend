using GlobalRide.Domain.Branches;
using GlobalRide.Domain.CarTypes;
using GlobalRide.Domain.Common.Result;

namespace GlobalRide.Domain.Rentals;

/// <summary>
/// Defines the contract for a service that handles one-way rental operations.
/// </summary>
public interface IOneWayRentalService
{
    /// <summary>
    /// Determines whether a rental is a one-way rental based on the pickup and dropoff branch identifiers.
    /// </summary>
    /// <param name="pickupBranchId">The unique identifier of the pickup branch.</param>
    /// <param name="dropoffBranchId">The unique identifier of the dropoff branch.</param>
    /// <returns><c>true</c> if the rental is a one-way rental (pickup and dropoff branches are different); otherwise, <c>false</c>.</returns>
    bool IsOneWayRental(Guid pickupBranchId, Guid dropoffBranchId);

    /// <summary>
    /// Checks the eligibility of a one-way rental based on the provided pickup and dropoff branches, rental period, and other constraints.
    /// </summary>
    /// <param name="pickupBranch">The pickup branch for the rental.</param>
    /// <param name="dropoffBranch">The dropoff branch for the rental.</param>
    /// <param name="period">The rental period, including the pickup and dropoff dates and times for the rental.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the rental is eligible. If not eligible, the result contains validation errors.
    /// </returns>
    Task<Result> CheckEligibilityAsync(
        Branch pickupBranch,
        Branch dropoffBranch,
        RentalPeriod period,
        CancellationToken cancellationToken);

    /// <summary>
    /// Calculates the one-way rental fee.
    /// </summary>
    /// <param name="pickupBranch">The pickup branch for the rental.</param>
    /// <param name="dropoffBranch">The dropoff branch for the rental.</param>
    /// <param name="carType">The car type associated with the rental.</param>
    /// <param name="pickupDate">The pickup date and time (in UTC) for the rental.</param>
    /// <returns>The total one-way rental fee as a <see cref="decimal"/>.</returns>
    decimal CalculateOneWayRentalFee(
        Branch pickupBranch,
        Branch dropoffBranch,
        CarType carType,
        DateTime pickupDate);
}
