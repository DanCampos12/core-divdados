using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class DeleteOperationCommand : Command<DeleteOperationCommandResult>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Id.ToString(), nameof(Id), "Id da operação é obrigatório")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));

        return Valid;
    }
}
