namespace GlobalRide.Api.Controllers.Cars;

public record class SearchCarsRequest(
    Guid PickupBranchId,
    Guid DropoffBranchId,
    DateTime PickupDateTime,
    DateTime DropoffDateTime);
