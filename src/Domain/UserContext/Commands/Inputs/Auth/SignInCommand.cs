using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class SignInCommand : Command<SignInCommandResult>
{
    public string Email { get; set; }
    public string Password { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Email, nameof(Email), "Email do usuário é obrigatório")
            .IsNotNullOrEmpty(Password, nameof(Password), "Senha do usuário é obrigatória"));

        return Valid;
    }
}
