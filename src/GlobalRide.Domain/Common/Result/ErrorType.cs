namespace GlobalRide.Domain.Common.Result;

/// <summary>
/// Represents the type of error that can occur in the application.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Indicates a validation error, typically caused by invalid or incorrect input data.
    /// </summary>
    Validation,

    /// <summary>
    /// Indicates that a requested resource or entity could not be found.
    /// </summary>
    NotFound,

    /// <summary>
    /// Indicates a conflict, such as when attempting to create or update a resource that already exists
    /// or violates a constraint.
    /// </summary>
    Conflict,
}
