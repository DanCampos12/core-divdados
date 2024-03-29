﻿using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class RecoverPasswordCommand : Command<RecoverPasswordCommandResult>
{
    public string Email { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Email, nameof(Email), "Email do usuário é obrigatório"));

        return Valid;
    }
}
