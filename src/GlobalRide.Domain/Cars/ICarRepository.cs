namespace GlobalRide.Domain.Cars;

/// <summary>
/// Defines the contract for a repository that provides data access operations for the <see cref="Car"/> entity.
/// </summary>
public interface ICarRepository
{
    /// <summary>
    /// Lists cars available at a specific branch within a given date range.
    /// </summary>
    /// <param name="pickupBranchId">The unique identifier of the pickup branch.</param>
    /// <param name="pickupDate">The pickup date and time in UTC.</param>
    /// <param name="dropoffDate">The dropoff date and time in UTC.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of available cars at the specified branch within the given date range.</returns>
    Task<IReadOnlyList<Car>> ListAvailableByBranchAndDatesAsync(
        Guid pickupBranchId,
        DateTime pickupDate,
        DateTime dropoffDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of cars associated with a specific branch.
    /// </summary>
    /// <param name="branchId">The unique identifier of the branch.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The number of cars associated with the specified branch.</returns>
    Task<int> CountByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of incoming cars from other branches to a specific branch on a given date.
    /// </summary>
    /// <param name="branchId">The unique identifier of the branch.</param>
    /// <param name="incomingDate">The date (in UTC) for which to count incoming cars.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The number of incoming cars to the specified branch on the given date.</returns>
    Task<int> CountIncomingByBranchIdAsync(Guid branchId, DateTime incomingDate, CancellationToken cancellationToken = default);
}
