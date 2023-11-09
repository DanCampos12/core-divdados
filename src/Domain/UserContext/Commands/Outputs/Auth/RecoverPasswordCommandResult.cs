using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class RecoverPasswordCommandResult : CommandResult
{
    public string Message { get; set; }
}