using GlobalRide.Domain.Common.Result;

using MediatR;

namespace GlobalRide.Application.Common.Messaging;

/// <summary>
/// Defines a command that represents a write operation.
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// Defines a command that represents a write operation and returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the command.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
