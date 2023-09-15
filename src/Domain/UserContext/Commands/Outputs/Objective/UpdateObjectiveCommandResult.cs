using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class UpdateObjectiveCommandResult : CommandResult
{
    public ObjectiveResult ObjectiveResult { get; set; }
}