namespace GlobalRide.Domain.AllowedCountryPairs;

/// <summary>
/// Defines the contract for a repository that provides data access operations for the <see cref="AllowedCountryPair"/> entity.
/// </summary>
public interface IAllowedCountryPairRepository
{
    /// <summary>
    /// Checks whether an allowed country pair exists between the specified pickup and dropoff country codes.
    /// </summary>
    /// <param name="pickupCountryCode">The country code representing the pickup location.</param>
    /// <param name="dropoffCountryCode">The country code representing the dropoff location.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// <c>true</c> if an allowed country pair exists between the specified pickup and dropoff country codes; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ExistsAsync(
        string pickupCountryCode,
        string dropoffCountryCode,
        CancellationToken cancellationToken = default);
}
