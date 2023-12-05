using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class ProcessObjectivesCommandResult : CommandResult
{
    public IEnumerable<ObjectiveResult> Objectives { get; set; }
}