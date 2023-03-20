using Flunt.Notifications;
using MediatR;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Core.Divdados.Shared.Commands;

public abstract class Handler<TCommand, TCommandResult> : Notifiable, IRequestHandler<TCommand, TCommandResult>
    where TCommand : Command<TCommandResult>
    where TCommandResult : CommandResult
{
    protected Task<TCommandResult> Complete(object response)
    {
        var commandResult = Activator.CreateInstance<TCommandResult>();
        commandResult.Response = response;
        return Task.FromResult(commandResult);
    }

    protected Task<TCommandResult> Complete(TCommandResult commandResult) =>
        Task.FromResult(commandResult);

    protected Task<TCommandResult> Incomplete()
    {
        var commandResult = Activator.CreateInstance<TCommandResult>();
        commandResult.AddNotifications(this);
        return Task.FromResult(commandResult);
    }

    public abstract Task<TCommandResult> Handle(TCommand command, CancellationToken cancellationToken);
}
