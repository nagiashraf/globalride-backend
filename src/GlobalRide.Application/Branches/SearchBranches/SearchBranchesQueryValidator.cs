using FluentValidation;

namespace GlobalRide.Application.Branches.SearchBranches;

/// <summary>
/// Validates instances of the <see cref="SearchBranchesQuery"/> class to ensure they meet the required criteria.
/// </summary>
public class SearchBranchesQueryValidator
    : AbstractValidator<SearchBranchesQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchBranchesQueryValidator"/> class and defines the validation rules.
    /// </summary>
    /// <remarks>
    /// The following rules are applied:
    /// - The search term must not be empty.
    /// - The maximum results count must be greater than 0 and less than 50.
    /// </remarks>
    public SearchBranchesQueryValidator()
    {
        RuleFor(q => q.SearchTerm).NotEmpty();
        RuleFor(q => q.MaxResultsCount).GreaterThan(0).LessThan(50);
    }
}
