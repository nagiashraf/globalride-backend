using FluentValidation;

using GlobalRide.Domain.Common.Result;

using MediatR;

namespace GlobalRide.Application.Common.Behaviors;

/// <summary>
/// A MediatR pipeline behavior that validates a request before passing it to the next handler in the pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled. Must implement <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response. Must inherit from <see cref="Result"/>.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    /// <summary>
    /// Handles the request by validating it before passing it to the next handler in the pipeline.
    /// </summary>
    /// <param name="request">The request to be validated and handled.</param>
    /// <param name="next">A delegate to the next handler in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Task{TResponse}"/> representing the result of the operation.
    /// If validation fails, the response will contain a list of validation errors.
    /// Otherwise, the response will be the result of the next handler in the pipeline.
    /// </returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null)
        {
            return await next();
        }

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = validationResult.Errors
            .ConvertAll(error => AppError.Validation(
                code: error.PropertyName,
                message: error.ErrorMessage));

        return (dynamic)errors;
    }
}
