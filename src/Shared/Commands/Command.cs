using Flunt.Notifications;
using MediatR;
using System;

namespace Core.Divdados.Shared.Commands;

public abstract class Command<TCommandResult> : Notifiable, IRequest<TCommandResult>
    where TCommandResult : CommandResult
{
    private bool _shouldCommit = true;
    private bool _shouldReturnResult = true;
    private bool _shouldPostProcess = true;

    public abstract bool Validate();
    public TCommandResult Result() => Activator.CreateInstance<TCommandResult>();

    public bool ShouldCommit => _shouldCommit;
    public bool ShouldReturnResult => _shouldReturnResult;
    public bool ShouldPostProcess => _shouldPostProcess;

    public Command<TCommandResult> DeactivateCommit()
    {
        _shouldCommit = false;
        _shouldReturnResult = false;
        return this;
    }

    public Command<TCommandResult> DeactivateReturnResult()
    {
        _shouldReturnResult = false;
        return this;
    }

    public Command<TCommandResult> DeactivatePostProcess()
    {
        _shouldPostProcess = false;
        return this;
    }
}
