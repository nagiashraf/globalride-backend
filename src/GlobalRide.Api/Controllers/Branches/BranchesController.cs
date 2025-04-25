using GlobalRide.Application.Branches.SearchBranches;
using GlobalRide.Domain.Common;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GlobalRide.Api.Controllers.Branches;

[Route("api/[controller]")]
public class BranchesController(ISender mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] SearchBranchesRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<LanguageCode>(request.LanguageCode, out var languageCode))
        {
            return BadRequest("Invalid language code.");
        }

        var query = new SearchBranchesQuery(
            request.SearchTerm,
            languageCode,
            request.ResultsCount);

        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            Ok,
            Problem);
    }
}
