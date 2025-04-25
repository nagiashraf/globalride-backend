using GlobalRide.Domain.Common;
using GlobalRide.Domain.Common.Result;

namespace GlobalRide.Domain.CarTypes;

/// <summary>
/// Represents a car type entity in the domain.
/// </summary>
public sealed class CarType : Entity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CarType"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the car type.</param>
    /// <param name="category">The category of the car type.</param>
    /// <param name="isOneWayDropoffAllowed">Indicates whether one-way dropoff is allowed for this car type.</param>
    /// <param name="oneWayFeeMultiplier">The fee multiplier applied for one-way dropoff, if applicable.</param>
    private CarType(
        Guid id,
        string category,
        bool isOneWayDropoffAllowed,
        decimal? oneWayFeeMultiplier)
        : base(id)
    {
        Category = category;
        IsOneWayDropoffAllowed = isOneWayDropoffAllowed;
        OneWayFeeMultiplier = oneWayFeeMultiplier;
    }

    private CarType()
    {
    }

    /// <summary>
    /// Gets the category of the car type.
    /// </summary>
    public string Category { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether one-way drop-off is allowed for this car type.
    /// </summary>
    public bool IsOneWayDropoffAllowed { get; private set; }

    /// <summary>
    /// Gets the fee multiplier applied for one-way drop-off, if applicable.
    /// </summary>
    public decimal? OneWayFeeMultiplier { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="CarType"/> class after validating the provided parameters.
    /// </summary>
    /// <param name="id">The unique identifier for the car type.</param>
    /// <param name="category">The category of the car type.</param>
    /// <param name="isOneWayDropoffAllowed">Indicates whether one-way drop-off is allowed for this car type.</param>
    /// <param name="oneWayFeeMultiplier">The fee multiplier applied for one-way drop-off, if applicable.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing either a valid <see cref="CarType"/> instance or an error if validation fails.
    /// </returns>
    public static Result<CarType> Create(
        Guid id,
        string category,
        bool isOneWayDropoffAllowed,
        decimal? oneWayFeeMultiplier)
    {
        if (isOneWayDropoffAllowed && !oneWayFeeMultiplier.HasValue)
        {
            return CarTypeErrors.OneWayFeeMultiplierNull;
        }

        if (!isOneWayDropoffAllowed && oneWayFeeMultiplier.HasValue)
        {
            return CarTypeErrors.OneWayFeeMultiplierNotNull;
        }

        return new CarType(
            id,
            category,
            isOneWayDropoffAllowed,
            oneWayFeeMultiplier);
    }
}
