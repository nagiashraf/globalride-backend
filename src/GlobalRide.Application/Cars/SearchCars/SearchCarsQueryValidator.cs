using FluentValidation;

namespace GlobalRide.Application.Cars.SearchCars;

/// <summary>
/// Validates instances of the <see cref="SearchCarsQuery"/> class to ensure they meet the required criteria.
/// </summary>
public sealed class SearchCarsQueryValidator
    : AbstractValidator<SearchCarsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchCarsQueryValidator"/> class and defines the validation rules.
    /// </summary>
    /// <remarks>
    /// The following rules are applied:
    /// - The pickup date must be earlier than the dropoff date.
    /// - The pickup date must be in the future.
    /// </remarks>
    public SearchCarsQueryValidator()
    {
        RuleFor(q => q.PickupDate).LessThan(q => q.DropoffDate);
        RuleFor(q => q.PickupDate).GreaterThan(DateTime.UtcNow).WithMessage("Pickup date must be in the future.");
    }
}
