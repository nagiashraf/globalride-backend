using GlobalRide.Domain.Common.Result;

namespace GlobalRide.Domain.CarTypes;

/// <summary>
/// Provides a collection of predefined validation errors specific to car type operations.
/// </summary>
public static class CarTypeErrors
{
    /// <summary>
    /// Represents a validation error that occurs when a one-way fee multiplier is provided but one-way dropoff is not allowed.
    /// </summary>
    public static readonly AppError OneWayFeeMultiplierNotNull = AppError.Validation(
        code: $"{nameof(CarType)}.",
        message: "One-way fee multiplier must be null if one-way dropoff is not allowed.");

    /// <summary>
    /// Represents a validation error that occurs when a one-way fee multiplier is not provided but one-way dropoff is allowed.
    /// </summary>
    public static readonly AppError OneWayFeeMultiplierNull = AppError.Validation(
        code: $"{nameof(CarType)}.",
        message: "One-way fee multiplier must not be null if one-way dropoff is allowed.");
}
