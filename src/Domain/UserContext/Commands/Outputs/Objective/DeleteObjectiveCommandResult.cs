using Core.Divdados.Shared.Commands;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class DeleteObjectiveCommandResult : CommandResult
{
    public Guid Id { get; set; }
}