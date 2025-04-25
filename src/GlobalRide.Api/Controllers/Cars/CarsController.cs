using GlobalRide.Application.Cars.SearchCars;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GlobalRide.Api.Controllers.Cars;

[Route("api/[controller]")]
public class CarsController(ISender mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> SearchAvailableByBranch(
        [FromQuery] SearchCarsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new SearchCarsQuery(
            request.PickupBranchId,
            request.DropoffBranchId,
            request.PickupDateTime,
            request.DropoffDateTime);

        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            Ok,
            Problem);
    }
}
