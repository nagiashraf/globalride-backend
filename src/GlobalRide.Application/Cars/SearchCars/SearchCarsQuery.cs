using GlobalRide.Application.Common.Messaging;

namespace GlobalRide.Application.Cars.SearchCars;

/// <summary>
/// Represents a query to search for available cars that are not rented and meet the specified criteria.
/// </summary>
/// <param name="PickupBranchId">The unique identifier of the branch where the car will be picked up.</param>
/// <param name="DropoffBranchId">The unique identifier of the branch where the car will be dropped off.</param>
/// <param name="PickupDate">The date and time (in UTC) when the car will be picked up.</param>
/// <param name="DropoffDate">The date and time (in UTC) when the car will be dropped off.</param>
public sealed record class SearchCarsQuery(
    Guid PickupBranchId,
    Guid DropoffBranchId,
    DateTime PickupDate,
    DateTime DropoffDate)
    : IQuery<IReadOnlyList<CarResponse>>;
