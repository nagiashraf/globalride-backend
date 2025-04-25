using GlobalRide.Application.Common.Messaging;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;
using GlobalRide.Domain.Common.Result;
using GlobalRide.Domain.Rentals;

namespace GlobalRide.Application.Cars.SearchCars;

/// <summary>
/// Handles the search for available cars based on the provided search criteria.
/// </summary>
/// <remarks>
/// This handler processes the <see cref="SearchCarsQuery"/> to find cars available for rental
/// between the specified pickup and dropoff branches and dates. It also handles one-way rental
/// scenarios by checking eligibility and calculating additional fees if applicable.
/// </remarks>
internal sealed class SearchCarsQueryHandler(
    IOneWayRentalService oneWayRentalService,
    IBranchRepository branchRepository,
    ICarRepository carRepository)
    : IQueryHandler<SearchCarsQuery, IReadOnlyList<CarResponse>>
{
    /// <summary>
    /// Handles the search for available cars based on the provided search criteria.
    /// </summary>
    /// <param name="request">The search query containing the pickup and dropoff branch IDs, dates, and other criteria.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A result containing a list of available cars that match the search criteria.</returns>
    public async Task<Result<IReadOnlyList<CarResponse>>> Handle(
        SearchCarsQuery request,
        CancellationToken cancellationToken)
    {
        var pickupBranch = await branchRepository.GetAsync(
            request.PickupBranchId,
            cancellationToken);

        if (pickupBranch is null)
        {
            return AppError.NotFound($"Pickup branch {request.PickupBranchId} not found.");
        }

        var dropoffBranch = pickupBranch;

        if (request.PickupBranchId != request.DropoffBranchId)
        {
            dropoffBranch = await branchRepository.GetAsync(
                request.DropoffBranchId,
                cancellationToken);
        }

        if (dropoffBranch is null)
        {
            return AppError.NotFound($"Dropoff branch {request.DropoffBranchId} not found.");
        }

        var rentalPeriod = new RentalPeriod(request.PickupDate, request.DropoffDate);
        if (rentalPeriod.Days > Rental.MaxRentalDays)
        {
            return AppError.Validation($"Rental period cannot exceed {Rental.MaxRentalDays} days.");
        }

        bool isOneWayRental = oneWayRentalService.IsOneWayRental(request.PickupBranchId, request.DropoffBranchId);
        if (isOneWayRental)
        {
            var eligibilityResult = await oneWayRentalService.CheckEligibilityAsync(
                pickupBranch,
                dropoffBranch,
                rentalPeriod,
                cancellationToken);

            if (eligibilityResult.IsFailure)
            {
                return eligibilityResult.Errors;
            }
        }

        var cars = await carRepository.ListAvailableByBranchAndDatesAsync(
            request.PickupBranchId,
            request.PickupDate,
            request.DropoffDate,
            cancellationToken);

        var response = cars.Select(c =>
        {
            if (c.CarType is null)
            {
                throw new InvalidOperationException("Car type cannot be null.");
            }

            decimal oneWayRentalFee = 0m;

            if (isOneWayRental)
            {
                oneWayRentalFee = oneWayRentalService.CalculateOneWayRentalFee(
                    pickupBranch,
                    dropoffBranch,
                    c.CarType,
                    request.PickupDate);
            }

            return new CarResponse(
            c.Id,
            c.Make,
            c.Model,
            c.Year,
            c.SeatsCount,
            c.CalculateTotalPrice(rentalPeriod, oneWayRentalFee),
            c.TransmissionType,
            c.FuelType);
        }).ToList();

        return Result.Success<IReadOnlyList<CarResponse>>(response);
    }
}
