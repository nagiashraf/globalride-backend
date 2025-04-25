using GlobalRide.Application.Common.Messaging;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Common.Result;

namespace GlobalRide.Application.Branches.SearchBranches;

/// <summary>
/// Handles the execution of the <see cref="SearchBranchesQuery"/> by searching for branches based on the provided criteria.
/// </summary>
internal sealed class SearchBranchesQueryHandler(IBranchSearchingService branchSearchingService)
    : IQueryHandler<SearchBranchesQuery, IReadOnlyList<BranchSearchResultResponse>>
{
    /// <summary>
    /// Handles the <see cref="SearchBranchesQuery"/> by delegating the search operation to the <see cref="IBranchSearchingService"/>
    /// and mapping the results to a list of <see cref="BranchSearchResultResponse"/> objects.
    /// </summary>
    /// <param name="request">The <see cref="SearchBranchesQuery"/> containing the search criteria.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a list of <see cref="BranchSearchResultResponse"/> objects representing the search results.
    /// </returns>
    public Task<Result<IReadOnlyList<BranchSearchResultResponse>>> Handle(
        SearchBranchesQuery request,
        CancellationToken cancellationToken)
    {
        var searchResults = branchSearchingService.Search(request.SearchTerm, request.LanguageCode, request.MaxResultsCount);
        var response = searchResults.Select(x => new BranchSearchResultResponse(
            x.Id,
            x.LanguageCode,
            x.Name,
            x.City,
            x.Country,
            x.Type,
            x.TimeZone)).ToList();

        return Task.FromResult(Result.Success<IReadOnlyList<BranchSearchResultResponse>>(response));
    }
}
