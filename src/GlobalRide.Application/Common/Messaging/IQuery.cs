using GlobalRide.Domain.Common.Result;

using MediatR;

namespace GlobalRide.Application.Common.Messaging;

/// <summary>
/// Defines a query that represents a read operation.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
