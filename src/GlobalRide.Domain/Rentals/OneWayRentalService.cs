#pragma warning disable CA1822
#pragma warning disable S2325

using GlobalRide.Domain.AllowedCountryPairs;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;
using GlobalRide.Domain.CarTypes;
using GlobalRide.Domain.Common.Result;

namespace GlobalRide.Domain.Rentals;

/// <summary>
/// Provides services for handling one-way rental operations.
/// </summary>
public sealed class OneWayRentalService(
    IAllowedCountryPairRepository allowedCountryPairRepository,
    ICarRepository carRepository)
    : IOneWayRentalService
{
    /// <summary>
    /// Determines whether a rental is a one-way rental based on the pickup and dropoff branch identifiers.
    /// </summary>
    /// <param name="pickupBranchId">The unique identifier of the pickup branch.</param>
    /// <param name="dropoffBranchId">The unique identifier of the dropoff branch.</param>
    /// <returns><c>true</c> if the rental is a one-way rental (pickup and dropoff branches are different); otherwise, <c>false</c>.</returns>
    public bool IsOneWayRental(Guid pickupBranchId, Guid dropoffBranchId) => pickupBranchId != dropoffBranchId;

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
    /// <remarks>
    /// This method performs the following checks:
    /// - Ensures the dropoff branch has sufficient capacity to accommodate the rental.
    /// - Validates that one-way dropoff is allowed at the dropoff branch.
    /// - Checks if the country pair (pickup and dropoff) is allowed for cross-border rentals.
    /// </remarks>
    public async Task<Result> CheckEligibilityAsync(
        Branch pickupBranch,
        Branch dropoffBranch,
        RentalPeriod period,
        CancellationToken cancellationToken)
    {
        var errors = new List<AppError>();

        int carsCountInDropoffBranch = await carRepository.CountByBranchIdAsync(
            dropoffBranch.Id,
            cancellationToken);

        int incomingCarsCountInDropoffBranchAtDropoffDate = await carRepository.CountIncomingByBranchIdAsync(
            dropoffBranch.Id,
            period.Dropoff,
            cancellationToken);

        int carsCountInDropoffBranchAtDropoffDate = carsCountInDropoffBranch + incomingCarsCountInDropoffBranchAtDropoffDate;
        if (dropoffBranch.Capacity <= carsCountInDropoffBranchAtDropoffDate)
        {
            errors.Add(RentalErrors.DropoffBranchCapacityExceeded);
        }

        if (!dropoffBranch.IsOneWayDropoffAllowed)
        {
            errors.Add(RentalErrors.DropoffBranchNotAllowed);
        }

        bool isCrossBorder = pickupBranch.CountryCode != dropoffBranch.CountryCode;
        if (isCrossBorder)
        {
            bool isAllowedCountryPair = await allowedCountryPairRepository.ExistsAsync(
                pickupBranch.CountryCode,
                dropoffBranch.CountryCode,
                cancellationToken);

            if (!isAllowedCountryPair)
            {
                errors.Add(RentalErrors.CountryPairNotAllowed);
            }
        }

        return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
    }

    /// <summary>
    /// Calculates the one-way rental fee based on the distance between branches, cross-border fees, and seasonal factors.
    /// </summary>
    /// <param name="pickupBranch">The pickup branch for the rental.</param>
    /// <param name="dropoffBranch">The dropoff branch for the rental.</param>
    /// <param name="carType">The car type associated with the rental.</param>
    /// <param name="pickupDate">The pickup date and time (in UTC) for the rental.</param>
    /// <returns>The total one-way rental fee as a <see cref="decimal"/>.</returns>
    /// <remarks>
    /// The fee calculation includes:
    /// - A base distance fee calculated using the distance between the pickup and dropoff branches.
    /// - A cross-border fee if the rental involves different countries.
    /// - A seasonal factor applied during peak season (December 25-31).
    /// - A multiplier based on the car type's one-way fee multiplier.
    /// </remarks>
    public decimal CalculateOneWayRentalFee(
        Branch pickupBranch,
        Branch dropoffBranch,
        CarType carType,
        DateTime pickupDate)
    {
        double distanceKmBetweenBranches = pickupBranch.Coordinate.CalculateDistanceKmTo(
            dropoffBranch.Coordinate);

        const decimal minimumFlatDistanceFee = 30m;
        const decimal ratePerKm = 0.2m;
        decimal distanceFee = minimumFlatDistanceFee + (decimal)distanceKmBetweenBranches * ratePerKm;

        decimal crossBorderFee = 0m;
        if (pickupBranch.CountryCode != dropoffBranch.CountryCode)
        {
            crossBorderFee = 100m;
        }

        decimal seasonalFactor = 1.0m;
        bool isDuringPeakSeason = pickupDate.Month == 12 && pickupDate.Day >= 25 && pickupDate.Day <= 31;
        if (isDuringPeakSeason)
        {
            seasonalFactor = 1.3m;
        }

        if (carType.OneWayFeeMultiplier is null)
        {
            throw new InvalidOperationException("Car type multiplier is null.");
        }

        decimal totalOneWayFee = (distanceFee + crossBorderFee) * (decimal)carType.OneWayFeeMultiplier * seasonalFactor;
        return totalOneWayFee;
    }
}
