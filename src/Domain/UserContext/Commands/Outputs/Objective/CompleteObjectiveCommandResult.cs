using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class CompleteObjectiveCommandResult : CommandResult
{
    public ObjectiveResult Objective { get; set; }
}