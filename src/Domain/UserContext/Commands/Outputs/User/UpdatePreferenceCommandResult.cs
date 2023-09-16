using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class UpdatePreferenceCommandResult : CommandResult
{
    public UserResult User { get; set; }
}