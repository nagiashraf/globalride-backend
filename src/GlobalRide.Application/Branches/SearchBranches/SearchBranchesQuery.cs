using GlobalRide.Application.Common.Messaging;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Common;

namespace GlobalRide.Application.Branches.SearchBranches;

/// <summary>
/// Represents a query to search for branches based on specific criteria.
/// </summary>
/// <param name="SearchTerm">The search term used to filter branches by name or other relevant fields.</param>
/// <param name="LanguageCode">The language code used for localization or filtering results based on language preferences.</param>
/// <param name="MaxResultsCount">The maximum number of results to return in the response.</param>
public sealed record class SearchBranchesQuery(
    string SearchTerm,
    LanguageCode LanguageCode,
    int MaxResultsCount)
    : IQuery<IReadOnlyList<BranchSearchResultResponse>>;
