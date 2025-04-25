using GlobalRide.Domain.Common.Result;

using MediatR;

namespace GlobalRide.Application.Common.Messaging;

/// <summary>
/// Defines a handler for queries that return a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the query.</typeparam>
public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
