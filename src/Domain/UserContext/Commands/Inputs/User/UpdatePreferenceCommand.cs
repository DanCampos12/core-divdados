using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class UpdatePreferenceCommand : Command<UpdatePreferenceCommandResult>
{
    public bool Dark { get; set; }
    public bool DisplayValues { get; set; }
    public Guid UserId { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório")
            .IsNotNull(Dark, nameof(Dark), "Tema é obrigatório")
            .IsNotNull(DisplayValues, nameof(DisplayValues), "Exibir valores é obrigatório"));

        return Valid;
    }
}