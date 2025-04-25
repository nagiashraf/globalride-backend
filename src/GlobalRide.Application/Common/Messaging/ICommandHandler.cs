using GlobalRide.Domain.Common.Result;

using MediatR;

namespace GlobalRide.Application.Common.Messaging;

/// <summary>
/// Defines a handler for commands that return no response.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

/// <summary>
/// Defines a handler for commands that return a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the command.</typeparam>
public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
