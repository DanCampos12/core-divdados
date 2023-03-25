using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class DeleteUserCommand : Command<DeleteUserCommandResult>
{
    public Guid Id { get; set; }
    public string Password { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Id.ToString(), nameof(Id), "Id do usuário é obrigatório")
            .IsNotNullOrEmpty(Password, nameof(Password), "Senha do usuário é obrigatória"));

        return Valid;
    }
}
