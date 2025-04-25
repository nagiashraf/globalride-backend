namespace GlobalRide.Domain.Common.Result;

/// <summary>
/// Represents an application error with a code, message, and type.
/// </summary>
/// <remarks>
/// This record class is immutable and provides factory methods for creating specific types of errors.
/// </remarks>
public sealed record class AppError
{
    private AppError(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// Gets the error code, which identifies the category or specific error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the error message, which describes the error in detail.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the type of error, as defined by the <see cref="ErrorType"/> enum.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Creates a validation error with the specified message and optional code.
    /// </summary>
    /// <param name="message">The error message describing the validation failure.</param>
    /// <param name="code">The error code, defaulting to "General.Validation".</param>
    /// <returns>A new <see cref="AppError"/> representing a validation error.</returns>
    public static AppError Validation(string message, string code = "General.Validation") =>
        new(code, message, ErrorType.Validation);

    /// <summary>
    /// Creates a not found error with the specified message and optional code.
    /// </summary>
    /// <param name="message">The error message describing the resource that was not found.</param>
    /// <param name="code">The error code, defaulting to "General.NotFound".</param>
    /// <returns>A new <see cref="AppError"/> representing a not found error.</returns>
    public static AppError NotFound(string message, string code = "General.NotFound") =>
        new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Creates a conflict error with the specified message and optional code.
    /// </summary>
    /// <param name="message">The error message describing the conflict.</param>
    /// <param name="code">The error code, defaulting to "General.Conflict".</param>
    /// <returns>A new <see cref="AppError"/> representing a conflict error.</returns>
    public static AppError Conflict(string message, string code = "General.Conflict") =>
        new(code, message, ErrorType.Conflict);
}
