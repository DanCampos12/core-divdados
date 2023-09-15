using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class CreateObjectiveCommandResult : CommandResult
{
    public ObjectiveResult Objective { get; set; }
}