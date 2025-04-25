using GlobalRide.Domain.Common;

namespace GlobalRide.Domain.Branches;

/// <summary>
/// Defines the contract for a service that provides full-text search functionality for branches.
/// </summary>
public interface IBranchSearchingService
{
    /// <summary>
    /// Adds a collection of branch search results to the search index.
    /// </summary>
    /// <param name="branches">The collection of branch search results to add to the index.</param>
    void AddRange(IEnumerable<BranchSearchResultResponse> branches);

    /// <summary>
    /// Searches for branches based on a search term and language code.
    /// </summary>
    /// <param name="searchTerm">The search term to query the index.</param>
    /// <param name="languageCode">The preferred language code for boosting search results.</param>
    /// <param name="maxResultsCount">The maximum number of results to return.</param>
    /// <returns>
    /// A list of <see cref="BranchSearchResultResponse"/> objects representing the search results.
    /// </returns>
    IReadOnlyList<BranchSearchResultResponse> Search(string searchTerm, LanguageCode languageCode, int maxResultsCount);
}
