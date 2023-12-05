using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class SignInCommandResult : CommandResult
{
    public UserResult User { get; set; }
    public string IdToken { get; set; }
}