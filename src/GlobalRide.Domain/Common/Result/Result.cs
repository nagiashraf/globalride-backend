namespace GlobalRide.Domain.Common.Result;

/// <summary>
/// Represents the result of an operation, which can either be a success or a failure.
/// </summary>
public class Result
{
    private readonly List<AppError>? _errors;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the result is a success.</param>
    /// <param name="errors">A list of errors if the result is a failure; otherwise, null.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the success/failure state does not match the presence of errors.
    /// </exception>
    protected Result(bool isSuccess, List<AppError>? errors)
    {
        if ((isSuccess && errors is not null) || (!isSuccess && (errors is null || errors.Count == 0)))
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        _errors = errors;
    }

    /// <summary>
    /// Gets a value indicating whether the result is a success.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result is a failure.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the errors associated with the result. Throws an exception if the result is a success.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to access the errors of a success result.
    /// </exception>
    public List<AppError> Errors => IsFailure
        ? _errors!
        : throw new InvalidOperationException(
            "The errors of a success result can not be accessed.");

    public static implicit operator Result(AppError error) => new(false, [error]);

    public static implicit operator Result(List<AppError> errors) => new(false, errors);

    /// <summary>
    /// Creates a successful result with no errors.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value associated with the success.</param>
    /// <returns>A successful <see cref="Result{TValue}"/>.</returns>
    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, null);

    /// <summary>
    /// Creates a failed result with a single error.
    /// </summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(AppError error) => new(false, [error]);

    /// <summary>
    /// Creates a failed result with a list of errors.
    /// </summary>
    /// <param name="errors">The list of errors associated with the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(List<AppError> errors) => new(false, errors);

    /// <summary>
    /// Creates a failed result with a single error and a default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A failed <see cref="Result{TValue}"/>.</returns>
    public static Result<TValue> Failure<TValue>(AppError error) =>
        new(default, false, [error]);

    /// <summary>
    /// Creates a failed result with a list of errors and a default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="errors">The list of errors associated with the failure.</param>
    /// <returns>A failed <see cref="Result{TValue}"/>.</returns>
    public static Result<TValue> Failure<TValue>(List<AppError> errors) =>
        new(default, false, errors);
}

/// <summary>
/// Represents the result of an operation that stores a value, which can either be a success or a failure.
/// </summary>
/// <typeparam name="TValue">The type of the value associated with the result.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value associated with the result.</param>
    /// <param name="isSuccess">Indicates whether the result is a success.</param>
    /// <param name="errors">A list of errors if the result is a failure; otherwise, null.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the value is null for a successful result.
    /// </exception>
    protected internal Result(TValue? value, bool isSuccess, List<AppError>? errors)
        : base(isSuccess, errors)
    {
        if (isSuccess && value is null)
        {
            throw new InvalidOperationException();
        }

        _value = value;
    }

    /// <summary>
    /// Gets the value associated with the result. Throws an exception if the result is a failure.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to access the value of a failed result.
    /// </exception>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException(
            "The value of a failure result can not be accessed.");

    public static implicit operator Result<TValue>(TValue? value) => new(value, true, null);

    public static implicit operator Result<TValue>(AppError error) => new(default, false, [error]);

    public static implicit operator Result<TValue>(List<AppError> errors) => new(default, false, errors);

    /// <summary>
    /// Matches the result to one of two functions based on whether it is a success or a failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the match.</typeparam>
    /// <param name="onSuccess">The function to execute if the result is a success.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<List<AppError>, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(_value!) : onFailure(Errors);
    }
}
