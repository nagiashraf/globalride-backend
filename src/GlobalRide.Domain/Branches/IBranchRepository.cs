namespace GlobalRide.Domain.Branches;

/// <summary>
/// Defines the contract for a repository that provides data access operations for the <see cref="Branch"/> entity.
/// </summary>
public interface IBranchRepository
{
    /// <summary>
    /// Lists all branch translations as a collection of <see cref="BranchSearchResultResponse"/> objects.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A list of <see cref="BranchSearchResultResponse"/> objects representing the translations of all branches.
    /// </returns>
    Task<IReadOnlyList<BranchSearchResultResponse>> ListTranslationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a branch by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the branch.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Branch"/> object representing the branch, or <c>null</c> if no branch is found with the specified identifier.
    /// </returns>
    Task<Branch?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
