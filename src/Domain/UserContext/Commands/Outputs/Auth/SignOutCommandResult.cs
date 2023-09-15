using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class SignOutCommandResult : CommandResult
{
    public Guid Id { get; set; }
}